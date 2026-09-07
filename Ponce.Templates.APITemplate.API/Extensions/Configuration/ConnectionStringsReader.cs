using Ponce.Templates.APITemplate.API.Helpers;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using Ponce.Templates.APITemplate.Infrastructure.Configurations;
using static Ponce.Templates.APITemplate.Domain.DomainConstants;

namespace Ponce.Templates.APITemplate.API.Extensions.Configuration;

public static partial class ConfigurationExtensions
{
    /// <summary>
    /// Reads a "DatabaseConnections:{sectionName}:ConnectionString" value (env var override
    /// supported), decrypting it on Windows if the appsettings value is protected.
    /// <para>
    /// This mirrors the original method almost verbatim. Two calls there depended on the
    /// company's internal packages and have no generic equivalent, so they're left as comments:
    /// <list type="bullet">
    /// <item>the original threw <c>Bisoft.Exceptions</c>' <c>TEnvironmentException.MissingConfiguration(...)</c>
    /// instead of the <see cref="ConfigurationException"/> used below;</item>
    /// <item><c>AppConfiguration.Unprotect(connectionString)</c> (from the company's
    /// "TechSoft.Configuration.EncriptacionAppSettings" package) decrypted a connection string that
    /// had been protected with Windows DPAPI via that same package's tooling. There is no generic
    /// stand-in for that - either bring the package back if you have access to it, or store the
    /// connection string unencrypted / use user-secrets / a secret manager instead.</item>
    /// </list>
    /// </para>
    /// </summary>
    private static string GetConnectionStringValue(this IConfiguration configuration, string sectionName)
    {
        var connectionString = configuration[$"DatabaseConnections:{sectionName}:ConnectionString"].TryOverwriteWithEnviromentValue($"{sectionName}_CONNECTION_STRING") ??
            throw ConfigurationException.MissingConfiguration(
                ExceptionSources.APPSETTINGS,
                $"Missing connection string '{sectionName}'"
            );
        // if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        //     connectionString = AppConfiguration.Unprotect(connectionString); // Bisoft/TechSoft-specific DPAPI decryption, see summary above.
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
