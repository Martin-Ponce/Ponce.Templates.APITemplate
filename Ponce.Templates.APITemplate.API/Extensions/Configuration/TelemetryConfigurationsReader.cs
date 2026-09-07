using Ponce.Templates.APITemplate.API.Dtos.Configurations;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.API.Extensions.Configuration;

public static partial class ConfigurationExtensions
{
    public static TelemetryConfiguration GetTelemetryConfiguration(this IConfiguration configuration)
    {
        return new TelemetryConfiguration()
        {
            Enabled = configuration.GetTelemetryEnabled(),
            TracesDestination = configuration.GetTelemetryDestination("Traces"),
            LogsDestination = configuration.GetTelemetryDestination("Logs"),
            LogsMinimumLevel = ParseLogEventLevel(configuration["Telemetry:LogsMinimumLevel"].TryOverwriteWithEnviromentValue("TELEMETRY_LOGS_MINIMUM_LEVEL"))
        };
    }

    private static bool GetTelemetryEnabled(this IConfiguration configuration)
    {
        var value = configuration["Telemetry:Enabled"].TryOverwriteWithEnviromentValue("TELEMETRY_ENABLED");
        return value.ToBool(ConfigurationException.InvalidConfiguration(
                ExceptionSources.APPSETTINGS,
                $"The 'Enabled' value of Telemetry must be a valid boolean, but '{value}' was received"
            ));
    }

    private static string GetTelemetryDestination(this IConfiguration configuration, string telemetryParameter)
    {
        var value = configuration[$"Telemetry:{telemetryParameter}Destination"].TryOverwriteWithEnviromentValue($"TELEMETRY_{telemetryParameter}_DESTINATION");
        return value.ToUrlWithoutSlash(ConfigurationException.InvalidConfiguration(
                ExceptionSources.APPSETTINGS,
                $"The '{telemetryParameter}Destination' value of Telemetry must be a valid URL, but '{value}' was received"
            ));
    }
}
