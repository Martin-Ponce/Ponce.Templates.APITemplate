using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Ponce.Templates.APITemplate.Infrastructure.Contexts;

namespace Ponce.Templates.APITemplate.API.Helpers.HealthChecks;

/// <summary>
/// The original project used the company's "Bisoft.DatabaseConnections" package
/// (<c>ConnectionStringValidatorFactory</c>) to validate connectivity without going through EF Core.
/// Plain EF Core's <see cref="DatabaseFacade.CanConnectAsync"/> is a generic equivalent.
/// </summary>
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
