using Microsoft.EntityFrameworkCore;
using Ponce.Templates.APITemplate.Infrastructure.Mapping.SqlServer;

namespace Ponce.Templates.APITemplate.Infrastructure.Helpers.Strategies;

internal class SqlServerConfigurationsStrategy : IDbConfigurationsStrategy
{
    public ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExampleItemSqlServerConfiguration());
        return modelBuilder;
    }
}
