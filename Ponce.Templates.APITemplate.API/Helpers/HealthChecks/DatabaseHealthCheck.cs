using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ponce.Templates.APITemplate.Infrastructure.Contexts;

namespace Ponce.Templates.APITemplate.API.Helpers.HealthChecks;

/// <summary>Checks that the database is reachable.</summary>
public class DatabaseHealthCheck(AppDbContext context) : IHealthCheck
{
    private readonly AppDbContext _context = context;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
        if (!canConnect)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, description: "Storage service unavailable.");
        }
        return new HealthCheckResult(HealthStatus.Healthy, description: "Storage service available.");
    }
}
