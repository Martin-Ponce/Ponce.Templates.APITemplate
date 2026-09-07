namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>
/// A required application/environment configuration value (appsettings, environment variables) is
/// missing or invalid. Thrown by the configuration-reading extensions in the API layer during startup.
/// </summary>
public class ConfigurationException : AppException
{
    public static readonly int MISSING_CONFIGURATION = 1;
    public static readonly int INVALID_CONFIGURATION = 2;

    public static ConfigurationException MissingConfiguration(string source, string message) =>
        new(MISSING_CONFIGURATION, message, new Dictionary<string, object> { { "Source", source } });
    public static ConfigurationException InvalidConfiguration(string source, string message) =>
        new(INVALID_CONFIGURATION, message, new Dictionary<string, object> { { "Source", source } });

    public ConfigurationException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
        : base(code, message, args) { }
}
