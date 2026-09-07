using Quartz;

namespace Ponce.Templates.APITemplate.API.BackgroundServices;

/// <summary>
/// Base class for a background service that runs on a Cron schedule.
/// <para>
/// The original project used the company's "Bisoft.AutomatedServices.Util" package for this
/// (a <c>TimedBackgroundService</c> base class driven by an <c>ITimedServiceConfiguration</c>).
/// This is a minimal generic replacement built on top of Quartz's <see cref="CronExpression"/>,
/// which the template already depends on for validating schedule strings at startup.
/// </para>
/// </summary>
public abstract class CronBackgroundService(string name, bool enabled, string cronSchedule, ILogger logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly string _name = name;
    private readonly bool _enabled = enabled;
    private readonly CronExpression _cronExpression = new(cronSchedule) { TimeZone = TimeZoneInfo.Local };
    private readonly ILogger _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected abstract Task ExecuteScheduledTask(IServiceScope scope, CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            _logger.LogInformation("Scheduled service {ServiceName} is disabled, skipping.", _name);
            return;
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            var nextOccurrence = _cronExpression.GetNextValidTimeAfter(DateTimeOffset.Now);
            if (nextOccurrence is null)
                break;

            var delay = nextOccurrence.Value - DateTimeOffset.Now;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, stoppingToken);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                _logger.LogDebug("Running scheduled service {ServiceName}.", _name);
                await ExecuteScheduledTask(scope, stoppingToken);
                _logger.LogDebug("Scheduled service {ServiceName} finished.", _name);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scheduled service {ServiceName} failed.", _name);
            }
        }
    }
}
