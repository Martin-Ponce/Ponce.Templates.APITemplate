using Serilog.Events;

namespace Ponce.Templates.APITemplate.API.Dtos.Configurations;

public class TelemetryConfiguration
{
    public bool Enabled { get; set; }
    public required string TracesDestination { get; set; }
    public required string LogsDestination { get; set; }
    public LogEventLevel LogsMinimumLevel { get; set; }
}
