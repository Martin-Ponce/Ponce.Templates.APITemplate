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
            AutomatedServices = configuration.GetAutomatedServicesConfigurations(),
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

    // The original project called `configuration.SetEncryption()` here, which relied on the
    // company's "TechSoft.Configuration.EncriptacionAppSettings" package to transparently decrypt
    // DPAPI-protected appsettings values on Windows. There is no generic equivalent - use
    // user-secrets/environment variables/a secret manager for sensitive configuration instead.
}
