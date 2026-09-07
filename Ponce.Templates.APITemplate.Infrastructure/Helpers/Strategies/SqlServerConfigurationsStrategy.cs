using Microsoft.EntityFrameworkCore;
using Ponce.Templates.APITemplate.Infrastructure.Mapping.SqlServer;

namespace Ponce.Templates.APITemplate.Infrastructure.Helpers.Strategies;

internal class SqlServerConfigurationsStrategy : IDbConfigurationsStrategy
{
    public ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ItemSqlServerConfiguration());
        // Add one modelBuilder.ApplyConfiguration(new <Entity>SqlServerConfiguration()) call per entity.
        return modelBuilder;
    }
}
