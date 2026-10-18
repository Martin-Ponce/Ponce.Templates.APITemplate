using Microsoft.EntityFrameworkCore;
using Ponce.Templates.APITemplate.Infrastructure.Mapping.Sqlite;

namespace Ponce.Templates.APITemplate.Infrastructure.Helpers.Strategies;

internal class SqliteConfigurationsStrategy : IDbConfigurationsStrategy
{
    public ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExampleItemSqliteConfiguration());
        return modelBuilder;
    }
}
