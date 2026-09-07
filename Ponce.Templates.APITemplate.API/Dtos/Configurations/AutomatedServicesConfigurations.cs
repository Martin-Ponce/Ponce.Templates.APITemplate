using Ponce.Templates.APITemplate.API.Dtos.Configurations.AutomatedServices;

namespace Ponce.Templates.APITemplate.API.Dtos.Configurations;

public class AutomatedServicesConfigurations(SampleScheduledJobConfiguration sampleScheduledJob)
{
    // Add one property + constructor parameter per scheduled/background service you register.
    public SampleScheduledJobConfiguration SampleScheduledJob { get; } = sampleScheduledJob;
}
