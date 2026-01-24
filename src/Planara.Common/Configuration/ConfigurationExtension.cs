using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Planara.Common.Configuration;

public static class ConfigurationExtension
{
    public static WebApplicationBuilder AddSettingsJson(this WebApplicationBuilder builder, string folder = "Settings")
    {
        builder.Configuration
            .AddJsonFile($"{folder}/appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"{folder}/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder;
    }
}