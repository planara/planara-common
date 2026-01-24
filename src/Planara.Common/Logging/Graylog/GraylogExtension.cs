using Microsoft.Extensions.Hosting;
using Serilog;

namespace Planara.Common.Logging.Graylog;

public static class GraylogExtension
{
    public static IHostBuilder UseGraylog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog(
            (context, config) => config.ReadFrom.Configuration(context.Configuration)
        );
    }
}