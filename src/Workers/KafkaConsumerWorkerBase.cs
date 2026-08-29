using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Planara.Kafka.Exceptions;
using Planara.Kafka.Interfaces;

namespace Planara.Common.Workers;

/// <summary>
/// Базовый фоновый воркер для обработки Kafka-сообщений
/// </summary>
public abstract class KafkaConsumerWorkerBase<TMessage>(ILogger logger, IKafkaConsumer<TMessage> consumer, IServiceScopeFactory scopeFactory) : BackgroundService where TMessage : class
{
    /// <summary>
    /// Логический ключ Kafka-топика
    /// </summary>
    protected abstract string TopicKey { get; }

    [ExcludeFromCodeCoverage]
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{WorkerName} started. Topic: {TopicKey}", GetType().Name, TopicKey);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ConsumeOnce(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("{WorkerName} cancellation requested.", GetType().Name);

                break;
            }
            catch (KafkaConsumeException exception)
            {
                logger.LogError(exception, "Failed to consume Kafka message in {WorkerName}.", GetType().Name);

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error in {WorkerName}.", GetType().Name);

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        try
        {
            consumer.Close();

            logger.LogInformation("{WorkerName} Kafka consumer closed.", GetType().Name);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error closing Kafka consumer in {WorkerName}.", GetType().Name);
        }
    }

    public async Task ConsumeOnce(CancellationToken cancellationToken)
    {
        var result = await consumer.ConsumeAsync(TopicKey, cancellationToken);

        if (result?.Message?.Value is not { } message)
            return;

        await using var scope = scopeFactory.CreateAsyncScope();

        await HandleMessage(message, scope.ServiceProvider, cancellationToken);

        await consumer.CommitAsync(result, cancellationToken);
    }

    /// <summary>
    /// Обрабатывает полученное Kafka-сообщение
    /// </summary>
    protected abstract Task HandleMessage(TMessage message, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}