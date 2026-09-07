using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using Ponce.Templates.APITemplate.Infrastructure.Contexts;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.Infrastructure.Configurations;

/// <summary>
/// Wires up <see cref="AppDbContext"/> against whichever provider is configured.
/// <para>
/// The original project used the company's "Bisoft.DatabaseConnections" package for this
/// (an <c>AddDbContext&lt;TContext&gt;(IDbConnectionConfiguration)</c> helper that also handled
/// provider-specific retry policies, an <c>EnvironmentDeployerFactory</c> for schema creation, and
/// a <c>ConnectionStringValidatorFactory</c> used by the health check). Since the template assumes
/// a single, plain EF Core database, this is a minimal generic replacement.
/// </para>
/// </summary>
public static class DbContextServiceExtensions
{
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, DatabaseConnectionConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddDbContext<AppDbContext>(options =>
        {
            switch (configuration.Provider)
            {
                case DatabaseProviders.SQL_SERVER:
                    options.UseSqlServer(configuration.ConnectionString);
                    break;
                case DatabaseProviders.POSTGRES:
                    options.UseNpgsql(configuration.ConnectionString);
                    break;
                case DatabaseProviders.SQLITE:
                    options.UseSqlite(configuration.ConnectionString);
                    break;
                default:
                    throw ConfigurationException.InvalidConfiguration(
                        ExceptionSources.APPSETTINGS,
                        $"Database provider '{configuration.Provider}' is not supported. Use one of: {DatabaseProviders.SQL_SERVER}, {DatabaseProviders.POSTGRES}, {DatabaseProviders.SQLITE}."
                    );
            }
        });
        return services;
    }
}
