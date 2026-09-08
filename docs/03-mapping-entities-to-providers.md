# Mapping Entities to the Database

This covers the **Infrastructure** layer: mapping an entity to a table for each supported database
provider, wiring that mapping into the provider-selection strategy, registering a `DbSet` for it, and
implementing its repository.

> Prerequisite: the entity, its validator, and its `I<Entity>Repository` contract already exist — see
> [Creating Entities](02-creating-entities.md) first if they don't.

This template supports three EF Core providers — SQL Server, PostgreSQL, SQLite — configurable per
environment via the `Provider` setting (see [Getting Started](01-getting-started.md#choosing-a-database-provider)).
Because the exact SQL types/casing/table naming conventions differ per engine, **every entity gets one
mapping class per provider** — nothing is left for EF Core to infer (no implicit column types, no implicit
max lengths, no implicit table names).

## 1. Add explicit column type constants (if you need a new one)

Each provider has its own small constants class under `Infrastructure/Mapping/ColumnTypes/`, holding the
literal SQL store-type names used by that provider's mappings:

```csharp
// Infrastructure/Mapping/ColumnTypes/SqlServerColumnTypes.cs
internal static class SqlServerColumnTypes
{
    public const string NVARCHAR = "nvarchar";
    public const string VARCHAR = "varchar";
    public const string BIT = "bit";
    public const string DATETIME2 = "datetime2";
    public const string UNIQUEIDENTIFIER = "uniqueidentifier";
}
```

Postgres's equivalent (`PostgresColumnTypes`) has `VARCHAR`, `BOOLEAN`, `TIMESTAMPTZ`, `UUID`. Sqlite's
(`SqliteColumnTypes`) has just `TEXT` and `INTEGER` (SQLite's storage classes are much coarser). Add a new
constant to the relevant class only if the type you need isn't already there.

## 2. Write one mapping class per provider

Under `Infrastructure/Mapping/<Provider>/`, one `internal class <Entity><Provider>Configuration : IEntityTypeConfiguration<TEntity>`
per provider. Every property gets an explicit `HasColumnName`, `HasColumnType`, and (for bounded strings)
`HasMaxLength` pulled from the `<Entity>Limits` class you added in
[Creating Entities](02-creating-entities.md#2-add-length-limits-and-if-needed-an-error-code).

Naming conventions used throughout this template (match them for consistency, they're not enforced by the
compiler):

| Provider | Table name | Column names |
|---|---|---|
| SqlServer | `PascalCase` (e.g. `ExampleItems`) | `camelCase` (e.g. `registeredAtUtc`) |
| Postgres | matches the entity's plural, as given (e.g. `ExampleItems`) | `snake_case` (e.g. `registered_at_utc`) |
| Sqlite | same as Postgres | `snake_case` |

SQL Server:

```csharp
// Infrastructure/Mapping/SqlServer/ExampleItemSqlServerConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.Templates.APITemplate.Domain.Entities.Example;
using static Ponce.Templates.APITemplate.Domain.DomainConstants.Values;
using static Ponce.Templates.APITemplate.Infrastructure.Mapping.ColumnTypes.SqlServerColumnTypes;

namespace Ponce.Templates.APITemplate.Infrastructure.Mapping.SqlServer;

internal class ExampleItemSqlServerConfiguration : IEntityTypeConfiguration<ExampleItem>
{
    public void Configure(EntityTypeBuilder<ExampleItem> builder)
    {
        builder.ToTable("ExampleItems")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType(UNIQUEIDENTIFIER)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(NVARCHAR)
            .HasMaxLength(ExampleItemLimits.MAX_LENGTH_NAME)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType(NVARCHAR)
            .HasMaxLength(ExampleItemLimits.MAX_LENGTH_DESCRIPTION)
            .IsRequired(false);
    }
}
```

Postgres and Sqlite follow the exact same shape — swap the `using static ...ColumnTypes` import, the
constants used (`UUID`/`VARCHAR` for Postgres, `TEXT` for Sqlite — Sqlite has no dedicated GUID or boolean
type, so both map to `TEXT`/`INTEGER` respectively), and the table casing per the table above. See
[`Infrastructure/Mapping/Postgres/ExampleItemPostgresConfiguration.cs`](../Ponce.Templates.APITemplate.Infrastructure/Mapping/Postgres/ExampleItemPostgresConfiguration.cs)
and [`.../Sqlite/ExampleItemSqliteConfiguration.cs`](../Ponce.Templates.APITemplate.Infrastructure/Mapping/Sqlite/ExampleItemSqliteConfiguration.cs)
for the full files.

## 3. Register the mapping in each provider's strategy

`Infrastructure/Helpers/Strategies/` has one class per provider implementing
[`IDbConfigurationsStrategy`](../Ponce.Templates.APITemplate.Infrastructure/Helpers/Strategies/IDbConfigurationsStrategy.cs):

```csharp
public interface IDbConfigurationsStrategy
{
    ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder);
}
```

Add one `modelBuilder.ApplyConfiguration(new <Entity><Provider>Configuration())` line to **each** of the
three strategy classes:

```csharp
// Infrastructure/Helpers/Strategies/SqlServerConfigurationsStrategy.cs
internal class SqlServerConfigurationsStrategy : IDbConfigurationsStrategy
{
    public ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExampleItemSqlServerConfiguration());
        // Add one modelBuilder.ApplyConfiguration(new <Entity>SqlServerConfiguration()) call per entity.
        return modelBuilder;
    }
}
```

...and the equivalent line (with the matching provider's configuration class) in
`PostgresConfigurationsStrategy.cs` and `SqliteConfigurationsStrategy.cs`.

Why per-provider strategy classes instead of one shared list? Because
[`AppDbContext`](../Ponce.Templates.APITemplate.Infrastructure/Contexts/AppDbContext.cs) only has **one**
`DbSet<ExampleItem>`, but three different `IEntityTypeConfiguration<ExampleItem>` implementations exist in
the same assembly (one per provider) — applying all of them at once would be a conflicting double
mapping. The strategy picks exactly one set, matching whichever provider is actually configured.

## 4. Let the factory pick the right strategy

[`DbConfigurationsStrategyFactory`](../Ponce.Templates.APITemplate.Infrastructure/Helpers/Factories/DbConfigurationsStrategyFactory.cs)
maps the configured provider string to its strategy — **you don't need to touch this file** when adding a
new entity, only when adding a whole new provider:

```csharp
internal static class DbConfigurationsStrategyFactory
{
    public static IDbConfigurationsStrategy GetStrategy(string provider) => provider switch
    {
        DatabaseProviders.SQL_SERVER => new SqlServerConfigurationsStrategy(),
        DatabaseProviders.POSTGRES => new PostgresConfigurationsStrategy(),
        DatabaseProviders.SQLITE => new SqliteConfigurationsStrategy(),
        _ => throw ConfigurationException.InvalidConfiguration(/* ... */)
    };
}
```

## 5. Add the `DbSet` to `AppDbContext`

```csharp
// Infrastructure/Contexts/AppDbContext.cs
public class AppDbContext(DbContextOptions<AppDbContext> options, DatabaseConnectionConfiguration configuration) : DbContext(options)
{
    public DbSet<ExampleItem> ExampleItems => Set<ExampleItem>();
    // Add one DbSet<TEntity> property per aggregate root.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DbConfigurationsStrategyFactory.GetStrategy(configuration.Provider).ApplyConfigurations(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
```

`configuration` here is the same `DatabaseConnectionConfiguration` read at startup (see
[Getting Started](01-getting-started.md#configuration)) — it's registered as a singleton and injected into
`AppDbContext`'s constructor by
[`DbContextServiceExtensions.AddAppDbContext`](../Ponce.Templates.APITemplate.Infrastructure/Extensions/DbContextServiceExtensions.cs),
which is what actually calls `.UseSqlServer(...)`/`.UseNpgsql(...)`/`.UseSqlite(...)` based on the same
provider value. You don't need to touch that file either when adding a new entity.

> **Table-creation caveat**: on startup, `Program.cs` calls `context.Database.EnsureCreatedAsync()`, which
> only creates tables if the database doesn't exist yet, or exists with zero tables — it will **not**
> retroactively add a table for a newly-added entity to a database that already has other tables in it.
> See the note in [Getting Started](01-getting-started.md#running-the-api).

## 6. Implement the repository

Under `Infrastructure/Repositories/`, implement the `I<Entity>Repository` contract from
[Creating Entities](02-creating-entities.md#5-write-the-repository-contract), deriving from the generic
[`Repository<TContext>`](../Ponce.Templates.APITemplate.Infrastructure/Repositories/Repository.cs) base
(which gives you `SaveChanges`/`BeginTransaction`/`CloseTransaction` for free):

```csharp
// Infrastructure/Repositories/ExampleItemRepository.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ponce.Templates.APITemplate.Domain.Contracts;
using Ponce.Templates.APITemplate.Domain.Entities.Example;
using Ponce.Templates.APITemplate.Infrastructure.Contexts;

namespace Ponce.Templates.APITemplate.Infrastructure.Repositories;

public class ExampleItemRepository(AppDbContext context, ILogger<ExampleItemRepository> logger)
    : Repository<AppDbContext>(context), IExampleItemRepository
{
    private readonly ILogger<ExampleItemRepository> _logger = logger;

    public IQueryable<ExampleItem> QueryExampleItems()
    {
        _logger.LogDebug("Querying example items");
        return _context.ExampleItems;
    }

    public async Task<ExampleItem?> GetExampleItemById(Guid exampleItemId, CancellationToken ct = default)
    {
        _logger.LogDebug("Fetching example item with id {ExampleItemId}", exampleItemId);
        return await _context.ExampleItems.FirstOrDefaultAsync(x => x.Id == exampleItemId, ct);
    }

    public async Task SaveExampleItem(ExampleItem exampleItem, CancellationToken ct = default)
    {
        _logger.LogDebug("Saving example item with id {ExampleItemId}", exampleItem.Id);
        await _context.ExampleItems.AddAsync(exampleItem, ct);
    }
}
```

`_context` (protected, from `Repository<TContext>`) is the same `AppDbContext` from step 5. Note that
`SaveExampleItem` only calls `AddAsync` — it doesn't call `SaveChanges`. That's intentional: the caller
(the application service, see next guide) decides when to commit.

## Next steps

The entity is now fully persisted, but nothing calls into it yet. Continue to
[Creating Endpoints](04-creating-endpoints.md) to build the application service and HTTP endpoints, and
to wire the repository and service into dependency injection.
