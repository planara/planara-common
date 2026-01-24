using Microsoft.Extensions.Hosting;
using Serilog;

namespace Planara.Common.Logging.Serilog;

public static class SerilogExtension
{
    public static IHostBuilder UseSerilog(this IHostBuilder builder)
    {
        return builder.UseSerilog(
            (context, config) => config.ReadFrom.Configuration(context.Configuration)
        );
    }
}