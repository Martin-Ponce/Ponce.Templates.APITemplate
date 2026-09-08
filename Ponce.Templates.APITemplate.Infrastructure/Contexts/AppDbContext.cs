using Microsoft.EntityFrameworkCore;
using Ponce.Templates.APITemplate.Domain.Entities.Example;
using Ponce.Templates.APITemplate.Infrastructure.Configurations;
using Ponce.Templates.APITemplate.Infrastructure.Helpers.Factories;

namespace Ponce.Templates.APITemplate.Infrastructure.Contexts;

/// <summary>
/// Single EF Core context for the template (the task assumes only one database is configured, with
/// the provider picked at startup via <see cref="DatabaseConnectionConfiguration.Provider"/>).
/// <para>
/// One <c>*Configuration</c> mapping class exists per entity per provider (explicit column
/// names/types/lengths, nothing inferred - see the <c>Mapping</c> folder), grouped by provider in
/// <see cref="Helpers.Strategies.IDbConfigurationsStrategy"/> implementations and selected here via
/// <see cref="DbConfigurationsStrategyFactory"/>.
/// </para>
/// </summary>
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
