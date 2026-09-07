using Microsoft.EntityFrameworkCore;
using Ponce.Templates.APITemplate.Domain.Entities.Sample;
using Ponce.Templates.APITemplate.Infrastructure.Configurations;
using Ponce.Templates.APITemplate.Infrastructure.Helpers.Factories;

namespace Ponce.Templates.APITemplate.Infrastructure.Contexts;

/// <summary>
/// Single EF Core context for the template (the task assumes only one database is configured, with
/// the provider picked at startup via <see cref="DatabaseConnectionConfiguration.Provider"/>).
/// <para>
/// The original project used the company's "Bisoft.DatabaseConnections" package for this (a
/// <c>ConfiguredDbContext&lt;TStrategyFactory&gt;</c> base class plus one <c>IModelBuilderConfigurations</c>
/// strategy per supported provider). This is a minimal generic replacement of that same shape: one
/// <c>*Configuration</c> mapping class per entity per provider (explicit column names/types/lengths,
/// nothing inferred - see the <c>Mapping</c> folder), grouped by provider in
/// <see cref="Helpers.Strategies.IDbConfigurationsStrategy"/> implementations and selected here via
/// <see cref="DbConfigurationsStrategyFactory"/>.
/// </para>
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options, DatabaseConnectionConfiguration configuration) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DbConfigurationsStrategyFactory.GetStrategy(configuration.Provider).ApplyConfigurations(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
