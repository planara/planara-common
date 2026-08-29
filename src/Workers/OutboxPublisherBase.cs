using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Planara.Common.Database.Domain;
using Planara.Kafka.Configurations;
using Planara.Kafka.Interfaces;

namespace Planara.Common.Workers;

public abstract class OutboxPublisherBase<TMessage>(IServiceScopeFactory scopeFactory, IKafkaProducer<TMessage> producer, ILogger logger) : BackgroundService
{
    private readonly string _workerId = $"{Environment.MachineName}:{Guid.NewGuid():N}";

    protected virtual int BatchSize => 50;

    protected virtual TimeSpan EmptyBatchInterval => TimeSpan.FromMilliseconds(300);

    protected virtual TimeSpan ErrorInterval => TimeSpan.FromSeconds(2);

    protected virtual TimeSpan LockDuration => TimeSpan.FromSeconds(30);

    [ExcludeFromCodeCoverage]
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await PublishOnce(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Outbox publisher crashed; retrying.");

                await Task.Delay(ErrorInterval, cancellationToken);
            }
        }
    }

    public async Task PublishOnce(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var type = typeof(TMessage).Name;

        await using var scope = scopeFactory.CreateAsyncScope();

        var dataContext = scope.ServiceProvider.GetRequiredService<DbContext>();
        var outboxMessages = dataContext.Set<OutboxMessage>();
        List<OutboxMessage> batch;

        await using (var transaction = await dataContext.Database.BeginTransactionAsync(cancellationToken))
        {
            batch = await outboxMessages
                .FromSqlRaw(
                    """
                    SELECT *
                    FROM "OutboxMessages"
                    WHERE "ProcessedAt" IS NULL
                      AND ("LockedUntil" IS NULL OR "LockedUntil" < {0})
                      AND "Type" = {1}
                    ORDER BY "CreatedAt", "Id"
                    FOR UPDATE SKIP LOCKED
                    LIMIT {2};
                    """,
                    now,
                    type,
                    BatchSize)
                .ToListAsync(cancellationToken);

            if (batch.Count == 0)
            {
                await transaction.CommitAsync(cancellationToken);
                await Task.Delay(EmptyBatchInterval, cancellationToken);

                return;
            }

            foreach (var message in batch)
            {
                message.LockedUntil = now.Add(LockDuration);
                message.LockedBy = _workerId;
                message.UpdatedAt = now;
            }

            await dataContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        foreach (var message in batch)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<TMessage>(message.PayloadJson, KafkaJson.DeserializerOptions)
                    ?? throw new InvalidOperationException("PayloadJson deserialized to null.");

                await producer.ProduceAsync(message.TopicKey, message.Key, payload, cancellationToken);

                var processedAt = DateTime.UtcNow;

                message.ProcessedAt = processedAt;
                message.UpdatedAt = processedAt;
                message.LastError = null;
                message.LockedUntil = null;
                message.LockedBy = null;
            }
            catch (Exception exception)
            {
                var failedAt = DateTime.UtcNow;

                message.AttemptCount++;
                message.LastAttemptAt = failedAt;

                var error = exception.ToString();
                message.LastError = error.Length > 4000 ? error[..4000] : error;

                var delaySeconds = Math.Min(60, 2 * message.AttemptCount);
                message.LockedUntil = failedAt.AddSeconds(delaySeconds);
                message.UpdatedAt = failedAt;

                logger.LogWarning(exception, "Failed to publish outbox message {Id}.", message.Id);
            }
        }

        await dataContext.SaveChangesAsync(cancellationToken);
    }
}