using Ponce.Templates.APITemplate.API.Dtos.Configurations;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.API.Extensions.Configuration;

public static partial class ConfigurationExtensions
{
    public static CorsConfiguration GetCorsConfiguration(this IConfiguration configuration)
    {
        return new CorsConfiguration(configuration.GetCorsOrigins(), configuration.GetCorsHeaders());
    }

    private static string[] GetCorsOrigins(this IConfiguration configuration)
    {
        var envValue = Environment.GetEnvironmentVariable("CORS_ORIGINS");
        if (!string.IsNullOrWhiteSpace(envValue))
        {
            return envValue.Split([','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        var origins = configuration.GetSection("Cors:Origins").Get<string[]>();
        if (origins == null || origins.Length <= 0)
            throw ConfigurationException.MissingConfiguration(
                ExceptionSources.APPSETTINGS,
                "The 'Cors:Origins' configuration is required and was not found."
            );

        return origins;
    }

    private static string[] GetCorsHeaders(this IConfiguration configuration)
    {
        var envValue = Environment.GetEnvironmentVariable("CORS_HEADERS");
        if (!string.IsNullOrWhiteSpace(envValue))
        {
            return envValue.Split([','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        var headers = configuration.GetSection("Cors:Headers").Get<string[]>();
        if (headers == null || headers.Length <= 0)
            throw ConfigurationException.MissingConfiguration(
                ExceptionSources.APPSETTINGS,
                "The 'Cors:Headers' configuration is required and was not found."
            );

        return headers;
    }
}
