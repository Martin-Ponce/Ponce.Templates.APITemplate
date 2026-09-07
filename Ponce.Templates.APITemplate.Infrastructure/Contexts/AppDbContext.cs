using Microsoft.EntityFrameworkCore;
using Ponce.Templates.APITemplate.Domain.Entities.Sample;

namespace Ponce.Templates.APITemplate.Infrastructure.Contexts;

/// <summary>
/// Single EF Core context for the template (the task assumes only one database is configured).
/// <para>
/// The original project used the company's "Bisoft.DatabaseConnections" package, which layered a
/// <c>ConfiguredDbContext&lt;TStrategyFactory&gt;</c> base class plus one <c>IModelBuilderConfigurations</c>
/// strategy per supported provider (SqlServer/Postgres/Sqlite) so the same context could target
/// different databases per environment. That is unnecessary complexity for a single-database
/// template, so entity mappings are applied directly from this assembly below.
/// </para>
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
