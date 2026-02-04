using HotChocolate.Fusion.Composition;
using HotChocolate.Fusion.Composition.Features;
using HotChocolate.Language;
using HotChocolate.Skimmed.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Planara.Common.GraphQL.Fusion;

public class GatewayConfigurationRedisObserver(
    IConfiguration configuration,
    IConnectionMultiplexer multiplexer,
    ILogger<GatewayConfigurationRedisObserver> logger
) : IObservable<GatewayConfiguration>
{
    public IDisposable Subscribe(IObserver<GatewayConfiguration> observer) =>
        new RedisConfigurationSession(observer, multiplexer, configuration, logger);

    private sealed class RedisConfigurationSession : IDisposable
    {
        private readonly IObserver<GatewayConfiguration> _observer;
        private readonly IConnectionMultiplexer _conn;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GatewayConfigurationRedisObserver> _logger;

        private string Name => _configuration["Name"] ?? string.Empty;

        public RedisConfigurationSession(
            IObserver<GatewayConfiguration> observer,
            IConnectionMultiplexer conn,
            IConfiguration configuration,
            ILogger<GatewayConfigurationRedisObserver> logger
        )
        {
            _observer = observer;
            _configuration = configuration;
            _logger = logger;
            _conn = conn;

            Task.Run(async () =>
                {
                    (
                        await conn.GetSubscriber().SubscribeAsync(WellKnownNames.PubSubChannel)
                    ).OnMessage(_ => BeginLoadConfig());
                })
                .ConfigureAwait(false);

            BeginLoadConfig();
        }

        private async Task<List<SubgraphConfiguration>> LoadSubgraphs()
        {
            var res = new List<SubgraphConfiguration>();

            _logger.LogInformation("Trying to get subgraphs by name {name}", Name);

            await foreach (var subgraphName in _conn.GetDatabase().SetScanAsync(Name))
            {
                _logger.LogInformation("Trying to get subgraph by name {name}", subgraphName.ToString());
                var configurationig = await LoadSubgraph(subgraphName.ToString());
                if (configurationig != null)
                {
                    res.Add(configurationig);
                }
            }

            return res;
        }

        private async Task<SubgraphConfiguration?> LoadSubgraph(string name)
        {
            try
            {
                var configurationigs = await _conn
                    .GetDatabase()
                    .HashGetAsync(Name + "." + name, [WellKnownFields.Schema]);

                if (configurationigs[0].IsNull)
                {
                    return null;
                }

                var schema = configurationigs[0].ToString();
                var uri = new Uri(_configuration[name + ":" + WellKnownFields.Uri]!);
                _logger.LogInformation("Loading {name} subgraph from {uri}", name, uri);
                IClientConfiguration client = uri.Scheme switch
                {
                    "http" or "https" => new HttpClientConfiguration(uri, "Fusion"),
                    "wss" => new WebSocketClientConfiguration(uri, "Fusion"),
                    _ => throw new Exception("Unknown uri scheme")
                };

                var ext = new List<string>();

                return new SubgraphConfiguration(
                    name,
                    schema,
                    ext,
                    new List<IClientConfiguration> { client },
                    default
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading {name} subgraph, skip", name);
                return null;
            }
        }

        private void BeginLoadConfig() =>
            Task.Run(async () =>
            {
                try
                {
                    var composer = new FusionGraphComposer();
                    var subgraphs = await LoadSubgraphs();

                    var fusionGraph = await composer.ComposeAsync(
                        subgraphs,
                        new FusionFeatureCollection()
                    );
                    var schema = SchemaFormatter.FormatAsString(fusionGraph);
                    if (schema.Length == 0)
                    {
                        throw new Exception("No subgraphs configurationigured");
                    }

                    var fusionGraphDoc = Utf8GraphQLParser.Parse(schema);

                    _observer.OnNext(new GatewayConfiguration(fusionGraphDoc));
                }
                catch (CompositionException ex)
                {
                    _logger.LogError("Failed to composite GraphQL schema: {Errors}", string.Join(", ", ex.Errors.Select(x => x.Message)));
                    _observer.OnError(ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to load GraphQL schema: {Error}", ex);
                    _observer.OnError(ex);
                }
            });

        public void Dispose() { }
    }
}
