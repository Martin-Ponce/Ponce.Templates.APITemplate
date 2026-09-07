using Ponce.Templates.APITemplate.API.Dtos.Configurations;
using Ponce.Templates.APITemplate.API.Dtos.Configurations.AutomatedServices;
using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using Quartz;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.API.Extensions.Configuration;

public static class AutomatedServicesConfigurationsReader
{
    public static AutomatedServicesConfigurations GetAutomatedServicesConfigurations(this IConfiguration configuration)
    {
        return new AutomatedServicesConfigurations(
            configuration.GetSampleScheduledJobConfiguration()
            // Add more automated service configurations here, one method + constructor parameter each.
        );
    }

    private static SampleScheduledJobConfiguration GetSampleScheduledJobConfiguration(this IConfiguration configuration)
    {
        const string serviceName = "SampleScheduledJob";
        return new SampleScheduledJobConfiguration(
            configuration.GetEnabledConfiguration(serviceName),
            configuration.GetScheduleConfiguration(serviceName)
        );
    }

    public static string GetScheduleConfiguration(this IConfiguration configuration, string serviceName)
    {
        var value = configuration[$"AutomatedServices:{serviceName}:Schedule"].TryOverwriteWithEnviromentValue($"{serviceName}_SCHEDULE");
        if (value is not null && CronExpression.IsValidExpression(value))
        {
            return value;
        }
        throw ConfigurationException.InvalidConfiguration(
            ExceptionSources.APPSETTINGS,
            $"The Schedule value of automated service {serviceName} must be a valid Cron expression, but '{value}' was received"
        );
    }

    public static bool GetEnabledConfiguration(this IConfiguration configuration, string serviceName)
    {
        var value = configuration[$"AutomatedServices:{serviceName}:Enabled"].TryOverwriteWithEnviromentValue($"{serviceName}_ENABLED");
        if (bool.TryParse(value, out bool enabled))
        {
            return enabled;
        }
        throw ConfigurationException.InvalidConfiguration(
            ExceptionSources.APPSETTINGS,
            $"The Enabled value of automated service {serviceName} must be a boolean, but '{value}' was received"
        );
    }
}
