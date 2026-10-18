using Microsoft.EntityFrameworkCore;
using Ponce.Templates.APITemplate.Domain.Entities.Example;
using Ponce.Templates.APITemplate.Infrastructure.Configurations;
using Ponce.Templates.APITemplate.Infrastructure.Helpers.Factories;

namespace Ponce.Templates.APITemplate.Infrastructure.Contexts;

/// <summary>Single EF Core context, the provider's mappings get picked at startup.</summary>
public class AppDbContext(DbContextOptions<AppDbContext> options, DatabaseConnectionConfiguration configuration) : DbContext(options)
{
    public DbSet<ExampleItem> ExampleItems => Set<ExampleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DbConfigurationsStrategyFactory.GetStrategy(configuration.Provider).ApplyConfigurations(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
