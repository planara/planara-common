using StackExchange.Redis;

namespace Planara.Common.GraphQL.Fusion;

public abstract class WellKnownFields
{
    public const string Schema = "schema";
    public const string Uri = "Uri";
}

public abstract class WellKnownNames
{
    public static readonly RedisChannel PubSubChannel = new(
        "FusionGatewayChannel",
        RedisChannel.PatternMode.Literal
    );
}
