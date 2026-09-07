namespace Ponce.Templates.APITemplate.API.Dtos.Configurations.AutomatedServices;

/// <summary>Config for the sample cron-scheduled background service. Copy for every real scheduled job.</summary>
public class SampleScheduledJobConfiguration(bool enabled, string schedule)
{
    public bool Enabled { get; } = enabled;
    public string Schedule { get; } = schedule;
}
