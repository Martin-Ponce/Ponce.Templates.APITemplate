using Serilog.Events;

namespace Ponce.Templates.APITemplate.API.Dtos.Configurations;

public class DbLoggerConfiguration(bool enabled, string databaseProvider, string databaseConnectionString, LogEventLevel minimumLevel)
{
    public bool Enabled { get; } = enabled;
    public string DatabaseProvider { get; } = databaseProvider;
    public string DatabaseConnectionString { get; } = databaseConnectionString;
    public LogEventLevel MinimumLevel { get; } = minimumLevel;
}
