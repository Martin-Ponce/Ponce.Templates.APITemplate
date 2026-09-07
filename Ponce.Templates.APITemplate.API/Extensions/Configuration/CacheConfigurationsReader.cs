using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using Ponce.Templates.APITemplate.Infrastructure.Configurations;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.API.Extensions.Configuration;

public static partial class ConfigurationExtensions
{
    public static CacheConfiguration GetCacheConfiguration(this IConfiguration configuration)
    {
        var cacheConfig = new CacheConfiguration
        {
            CacheEnabled = configuration.GetCacheEnabled(),
            CacheSlidingDuration = configuration.GetCacheDurationConfiguration("Cache:SlidingExpirationInMinutes", "CACHE_SLIDING_DURATION"),
            CacheAbsoluteDuration = configuration.GetCacheDurationConfiguration("Cache:AbsoluteExpirationInMinutes", "CACHE_ABSOLUTE_DURATION")
        };
        if (cacheConfig.CacheSlidingDuration > cacheConfig.CacheAbsoluteDuration)
        {
            throw ConfigurationException.InvalidConfiguration(
                ExceptionSources.APPSETTINGS,
                $"The cache's sliding duration ({cacheConfig.CacheSlidingDuration.TotalMinutes} minutes) cannot be greater than its absolute duration ({cacheConfig.CacheAbsoluteDuration.TotalMinutes} minutes)."
            );
        }
        return cacheConfig;
    }

    private static TimeSpan GetCacheDurationConfiguration(this IConfiguration configuration, string configKey, string envKey)
    {
        var value = configuration[configKey].TryOverwriteWithEnviromentValue(envKey);
        if (int.TryParse(value, out int minutes) && minutes > 0)
        {
            return TimeSpan.FromMinutes(minutes);
        }
        throw ConfigurationException.InvalidConfiguration(
            ExceptionSources.APPSETTINGS,
            $"The cache duration value must be a positive integer representing minutes, but '{value}' was received"
        );
    }

    private static bool GetCacheEnabled(this IConfiguration configuration)
    {
        var value = configuration["Cache:Enabled"].TryOverwriteWithEnviromentValue("CACHE_ENABLED");
        return value.ToBool(ConfigurationException.InvalidConfiguration(
                ExceptionSources.APPSETTINGS,
                $"The 'Enabled' value of Cache must be a valid boolean, but '{value}' was received"
        ));
    }
}
