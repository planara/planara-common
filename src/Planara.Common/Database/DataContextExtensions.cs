using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Planara.Common.Database;

public static partial class DataContextExtensions
{
    private const string Pattern = @"(?<=Database=)[^;]*";
    private const string PGDATABASE = nameof(PGDATABASE);

    public static IServiceCollection AddDataContext<TContext>(
        this IServiceCollection services,
        string connectionString,
        int maxRetryCount = 6,
        int maxDelay = 30,
        bool enableSensitiveDataLogging = false
    )
        where TContext: DbContext
    {
        var dbName = Environment.GetEnvironmentVariable(PGDATABASE);
        if (!string.IsNullOrEmpty(dbName))
            connectionString = Regex().Replace(connectionString, dbName);

        services.AddPooledDbContextFactory<TContext>(builder =>
            builder
                .EnableSensitiveDataLogging(enableSensitiveDataLogging)
                .UseNpgsql(
                    connectionString,
                    _ => { }
                )
        );

        services.AddDbContextPool<DbContext, TContext>(builder =>
            builder
                .EnableSensitiveDataLogging(enableSensitiveDataLogging)
                .UseNpgsql(
                    connectionString,
                    _ => { }
                )
        );

        return services;
    }

    [GeneratedRegex(Pattern)]
    private static partial Regex Regex();
}
