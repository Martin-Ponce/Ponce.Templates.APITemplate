using Microsoft.EntityFrameworkCore;
using Ponce.Templates.APITemplate.API.Dtos.Configurations.AutomatedServices;
using Ponce.Templates.APITemplate.Domain.Services;

namespace Ponce.Templates.APITemplate.API.BackgroundServices;

/// <summary>
/// Sample cron-scheduled background service. Copy this shape (constructor + ExecuteScheduledTask)
/// for every real scheduled job, then delete this sample.
/// </summary>
public class SampleScheduledJob(
    ILogger<SampleScheduledJob> logger,
    IServiceScopeFactory scopeFactory,
    SampleScheduledJobConfiguration configuration)
    : CronBackgroundService(nameof(SampleScheduledJob), configuration.Enabled, configuration.Schedule, logger, scopeFactory)
{
    protected override async Task ExecuteScheduledTask(IServiceScope scope, CancellationToken stoppingToken)
    {
        var itemDomainService = scope.ServiceProvider.GetRequiredService<ItemDomainService>();
        var itemCount = await itemDomainService.QueryItems().CountAsync(stoppingToken);
        scope.ServiceProvider.GetRequiredService<ILogger<SampleScheduledJob>>()
            .LogInformation("There are currently {ItemCount} items registered.", itemCount);
    }
}
