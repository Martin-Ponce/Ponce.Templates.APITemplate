using Microsoft.EntityFrameworkCore;
using Ponce.Templates.APITemplate.Infrastructure.Mapping.Postgres;

namespace Ponce.Templates.APITemplate.Infrastructure.Helpers.Strategies;

internal class PostgresConfigurationsStrategy : IDbConfigurationsStrategy
{
    public ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ItemPostgresConfiguration());
        // Add one modelBuilder.ApplyConfiguration(new <Entity>PostgresConfiguration()) call per entity.
        return modelBuilder;
    }
}
