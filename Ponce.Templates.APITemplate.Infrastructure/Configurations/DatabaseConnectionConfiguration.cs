namespace Ponce.Templates.APITemplate.Infrastructure.Configurations;

/// <summary>Provider name plus connection string.</summary>
public class DatabaseConnectionConfiguration(string provider, string connectionString)
{
    public string Provider { get; } = provider;
    public string ConnectionString { get; } = connectionString;
}

/// <summary>Supported provider names, matched against the "Provider" value in appsettings.</summary>
public static class DatabaseProviders
{
    public const string SQL_SERVER = "SqlServer";
    public const string POSTGRES = "Postgres";
    public const string SQLITE = "Sqlite";
}
