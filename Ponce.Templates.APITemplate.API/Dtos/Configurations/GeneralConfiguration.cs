using Ponce.Templates.APITemplate.Infrastructure.Configurations;

namespace Ponce.Templates.APITemplate.API.Dtos.Configurations;

public class GeneralConfiguration
{
    public required DatabaseConnectionConfiguration Database { get; set; }
    public required GeneralLoggerConfiguration Logger { get; set; }
    public required TelemetryConfiguration Telemetry { get; set; }
    public required CacheConfiguration Cache { get; set; }
    public required AutomatedServicesConfigurations AutomatedServices { get; set; }
    public required CorsConfiguration Cors { get; set; }
    public required int RateLimiterMaxCalls { get; set; }
}
