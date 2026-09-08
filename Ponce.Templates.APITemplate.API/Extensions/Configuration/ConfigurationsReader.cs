using Ponce.Templates.APITemplate.API.Dtos.Configurations;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using Ponce.Templates.APITemplate.Domain.Validators;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.API.Extensions.Configuration;

public static partial class ConfigurationExtensions
{
    private const string DATABASE_CONTEXT_CONNECTION_STRING_NAME = "Database";

    public static GeneralConfiguration GetConfiguration(this IConfiguration configuration)
    {
        var databaseConfiguration = configuration.GetConnectionConfiguration(DATABASE_CONTEXT_CONNECTION_STRING_NAME);
        return new GeneralConfiguration()
        {
            Database = databaseConfiguration,
            Logger = configuration.GetGeneralLoggerConfiguration(databaseConfiguration),
            RateLimiterMaxCalls = configuration.GetRateLimiterMaxCalls(),
            Telemetry = configuration.GetTelemetryConfiguration(),
            Cache = configuration.GetCacheConfiguration(),
            Cors = configuration.GetCorsConfiguration(),
        };
    }

    private static int GetRateLimiterMaxCalls(this IConfiguration configuration)
    {
        var value = configuration["MaxCallsPerMinute"].TryOverwriteWithEnviromentValue("MAX_CALLS");
        value.ValidateNull(ConfigurationException.MissingConfiguration(
                ExceptionSources.APPSETTINGS,
                "Missing maximum amount of calls"
            ));
        if (!int.TryParse(value, out var result))
            throw ConfigurationException.InvalidConfiguration(
                ExceptionSources.APPSETTINGS,
                "The maximum amount of calls has an invalid value"
            );
        return result;
    }

    // Sensitive configuration (e.g. connection strings) should be supplied via user-secrets,
    // environment variables, or a secret manager rather than stored in plain appsettings.json.
}
