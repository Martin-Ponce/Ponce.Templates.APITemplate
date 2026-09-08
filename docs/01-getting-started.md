# Getting Started

## Prerequisites

- .NET 10 SDK
- One of: SQL Server, PostgreSQL, or SQLite (SQLite needs nothing installed — it's a local file)

## Project layout

```
Ponce.Templates.APITemplate.Domain/          Entities, validators, repository contracts, exceptions
Ponce.Templates.APITemplate.Application/     DTOs, application services
Ponce.Templates.APITemplate.Infrastructure/  DbContext, EF Core mappings, repositories
Ponce.Templates.APITemplate.API/             Minimal-API endpoints, DI wiring, appsettings
```

See the [README](README.md) for the full architecture diagram and request flow.

## Configuration

All configuration is read once at startup in `Program.cs`, via `builder.Configuration.GetConfiguration()`
(defined in [`Extensions/Configuration/ConfigurationsReader.cs`](../Ponce.Templates.APITemplate.API/Extensions/Configuration/ConfigurationsReader.cs)),
which builds a single `GeneralConfiguration` object out of several sections. Each section has its own
`Get<Section>Configuration()` extension method in `Extensions/Configuration/`.

### appsettings.json

The base configuration lives in [`appsettings.json`](../Ponce.Templates.APITemplate.API/appsettings.json):

```json
{
  "DatabaseConnections": {
    "Database": {
      "ConnectionString": "Server=ServerName;Initial Catalog=DatabaseName;User Id=User;Password=Password;Trusted_Connection=False;Encrypt=False;TrustServerCertificate=True;",
      "Provider": "SqlServer"
    }
  },
  "Logger": { "LogHttpRequests": false, "Database": { "Enabled": false, "MinimumLevel": "Error" } },
  "Telemetry": { "Enabled": false, "TracesDestination": "...", "LogsDestination": "...", "LogsMinimumLevel": "Error" },
  "MaxCallsPerMinute": 100,
  "Cache": { "Enabled": false, "SlidingExpirationInMinutes": 15, "AbsoluteExpirationInMinutes": 60 },
  "Cors": { "Origins": ["http://localhost:4200"], "Headers": ["X-Pagination"] }
}
```

`appsettings.{Environment}.json` (e.g. `appsettings.Development.json`) layers on top of this as usual in
ASP.NET Core, and in the `Development` environment, **user secrets are layered on top of that** (the API
project already has a `UserSecretsId` in its `.csproj`). That means locally, the connection string you
actually get at runtime is whatever's in user secrets, if anything is set there — check with:

```powershell
dotnet user-secrets list --project Ponce.Templates.APITemplate.API
```

To point your local run at your own database:

```powershell
dotnet user-secrets set "DatabaseConnections:Database:Provider" "SqlServer" --project Ponce.Templates.APITemplate.API
dotnet user-secrets set "DatabaseConnections:Database:ConnectionString" "Server=.;Initial Catalog=MyLocalDb;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True" --project Ponce.Templates.APITemplate.API
```

### Choosing a database provider

The `Provider` value must be one of the constants in [`Infrastructure/Configurations/DatabaseConnectionConfiguration.cs`](../Ponce.Templates.APITemplate.Infrastructure/Configurations/DatabaseConnectionConfiguration.cs):

```csharp
public static class DatabaseProviders
{
    public const string SQL_SERVER = "SqlServer";
    public const string POSTGRES = "Postgres";
    public const string SQLITE = "Sqlite";
}
```

Whatever you pick, entities must have a mapping for that provider — see
[Mapping Entities to the Database](03-mapping-entities-to-providers.md).

### Environment-variable overrides

Most configuration readers support overriding a single value via an environment variable, without
touching `appsettings.json` (handy for containers/CI). The variable name is **specific to each setting**
and defined right where it's read — for example:

- `Extensions/Configuration/ConnectionStringsReader.cs`: `Database_CONNECTION_STRING`, `Database_DATABASE_PROVIDER`
- `Extensions/Configuration/CacheConfigurationsReader.cs`: `CACHE_ENABLED`, `CACHE_SLIDING_DURATION`, `CACHE_ABSOLUTE_DURATION`
- `Extensions/Configuration/ConfigurationsReader.cs`: `MAX_CALLS`

Check the relevant `*ConfigurationsReader.cs` file for the exact name when you need one — there's no
single universal naming rule across all settings. The lookup itself is case-insensitive
(`EnvironmentVariablesReader.TryOverwriteWithEnviromentValue` upper-cases the name before reading it).

## Running the API

```powershell
cd Ponce.Templates.APITemplate.API
dotnet run
```

This uses the `http` launch profile in `Properties/launchSettings.json` (`http://localhost:5008`,
`ASPNETCORE_ENVIRONMENT=Development`). On startup the app calls `EnsureDatabaseCreated` (in `Program.cs`),
which creates the configured database **only if it doesn't exist yet, or exists but has zero tables**. If
the database already has at least one table, it does nothing further — it will not create tables for
newly-added entities. That's a deliberate placeholder (see the comment above `EnsureDatabaseCreated` in
`Program.cs`); moving to EF Core Migrations is the natural next step once you need to evolve a live
database's schema, but is outside the scope of this template as-is.

Once running:

- Swagger UI: `http://localhost:5008/swagger` (only mapped in `Development`, see `UseVersionedSwagger` in `WebApplicationExtensions.cs`)
- Welcome/health page: `http://localhost:5008/`
- API root: `http://localhost:5008/api/...` (see [Creating Endpoints](04-creating-endpoints.md) for how routes under `api/` are built)

Requests need an `X-Version: 1.0` header (or whatever versions you've added — see `ApiConstants.VERSION_1`
and `ConfigureApiVersioning` in `ServiceExtensions.cs`) since the API is version-aware from the start.

## Where to go next

- Adding a whole new feature? Start at [Creating Entities](02-creating-entities.md).
- Just need to add an endpoint to something that already has an entity/mapping? Skip to [Creating Endpoints](04-creating-endpoints.md).
