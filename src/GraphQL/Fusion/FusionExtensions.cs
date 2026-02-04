using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Planara.Common.GraphQL.Fusion;

public static class FusionExtensions
{
    public static FusionGatewayBuilder UseSchemaFromRedis(
        this FusionGatewayBuilder builder,
        IConfiguration configuration
    )
    {
        builder.RegisterGatewayConfiguration(sp => new GatewayConfigurationRedisObserver(
            configuration,
            sp.GetRequiredService<IConnectionMultiplexer>(),
            sp.GetRequiredService<ILogger<GatewayConfigurationRedisObserver>>()
        ));

        return builder;
    }

    public static FusionGatewayBuilder UseSchemaFromRedis(
        this FusionGatewayBuilder builder,
        Func<IServiceProvider, IConnectionMultiplexer> connectionFactory,
        IConfiguration configuration
    )
    {
        builder.RegisterGatewayConfiguration(sp => new GatewayConfigurationRedisObserver(
            configuration,
            connectionFactory(sp),
            sp.GetRequiredService<ILogger<GatewayConfigurationRedisObserver>>()
        ));

        return builder;
    }
    
    public static IRequestExecutorBuilder PublishSchemaToRedis(
        this IRequestExecutorBuilder builder,
        string configurationName,
        string name
    )
    {
        builder.ConfigureOnRequestExecutorCreatedAsync(
            async (sp, executor, _) =>
            {
                var conn = sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase();
                var schema = executor.Schema.Print();
                await conn.SetAddAsync(configurationName, name);
                await conn.HashSetAsync(
                    configurationName + "." + name,
                    WellKnownFields.Schema,
                    schema
                );
                await conn.PublishAsync(WellKnownNames.PubSubChannel, "updateNotification");
            }
        );

        return builder;
    }

    public static IRequestExecutorBuilder PublishSchemaToRedis(
        this IRequestExecutorBuilder builder,
        Func<IServiceProvider, IConnectionMultiplexer> connectionFactory,
        string configurationName,
        string name
    )
    {
        builder.ConfigureOnRequestExecutorCreatedAsync(
            async (sp, executor, _) =>
            {
                var conn = connectionFactory(sp).GetDatabase();
                var schema = executor.Schema.Print();
                await conn.SetAddAsync(configurationName, name);
                await conn.HashSetAsync(
                    configurationName + "." + name,
                    WellKnownFields.Schema,
                    schema
                );
                await conn.PublishAsync(WellKnownNames.PubSubChannel, "updateNotification");
            }
        );

        return builder;
    }
}