namespace Ponce.Templates.APITemplate.API.Dtos.Configurations;

public class GeneralLoggerConfiguration
{
    public bool LogHttpRequests { get; set; }
    /// <summary>Optional sink that writes log events to the same database as the application (disabled by default).</summary>
    public required DbLoggerConfiguration Database { get; set; }
}
