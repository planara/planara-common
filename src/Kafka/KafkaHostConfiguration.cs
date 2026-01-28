using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Planara.Kafka.Interfaces;

namespace Planara.Common.Kafka;

public static class KafkaHostConfiguration
{
    public static async Task UseKafka(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<IKafkaTopicsInitializer>();
            await initializer.EnsureTopicsCreatedAsync();
        }
    }
}