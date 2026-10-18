using Ponce.Templates.APITemplate.Domain.Exceptions;
using Ponce.Templates.APITemplate.Infrastructure.Configurations;
using Ponce.Templates.APITemplate.Infrastructure.Helpers.Strategies;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.Infrastructure.Helpers.Factories;

/// <summary>Picks the mapping strategy for the configured provider.</summary>
internal static class DbConfigurationsStrategyFactory
{
    public static IDbConfigurationsStrategy GetStrategy(string provider) => provider switch
    {
        DatabaseProviders.SQL_SERVER => new SqlServerConfigurationsStrategy(),
        DatabaseProviders.POSTGRES => new PostgresConfigurationsStrategy(),
        DatabaseProviders.SQLITE => new SqliteConfigurationsStrategy(),
        _ => throw ConfigurationException.InvalidConfiguration(
            ExceptionSources.APPSETTINGS,
            $"Database provider '{provider}' is not supported. Use one of: {DatabaseProviders.SQL_SERVER}, {DatabaseProviders.POSTGRES}, {DatabaseProviders.SQLITE}."
        )
    };
}
