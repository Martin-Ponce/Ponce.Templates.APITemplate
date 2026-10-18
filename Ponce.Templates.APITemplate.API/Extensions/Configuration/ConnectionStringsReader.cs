using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using Ponce.Templates.APITemplate.Infrastructure.Configurations;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.API.Extensions.Configuration;

public static partial class ConfigurationExtensions
{
    private static string GetConnectionStringValue(this IConfiguration configuration, string sectionName)
    {
        var connectionString = configuration[$"DatabaseConnections:{sectionName}:ConnectionString"].TryOverwriteWithEnviromentValue($"{sectionName}_CONNECTION_STRING") ??
            throw ConfigurationException.MissingConfiguration(
                ExceptionSources.APPSETTINGS,
                $"Missing connection string '{sectionName}'"
            );
        return connectionString;
    }

    private static DatabaseConnectionConfiguration GetConnectionConfiguration(this IConfiguration configuration, string sectionName)
    {
        var connectionString = configuration.GetConnectionStringValue(sectionName);
        var provider = configuration[$"DatabaseConnections:{sectionName}:Provider"].TryOverwriteWithEnviromentValue($"{sectionName}_DATABASE_PROVIDER") ??
            throw ConfigurationException.MissingConfiguration(ExceptionSources.APPSETTINGS, "Missing database provider configuration");
        return new DatabaseConnectionConfiguration(provider, connectionString);
    }
}
