using Microsoft.EntityFrameworkCore;

namespace Ponce.Templates.APITemplate.Infrastructure.Helpers.Strategies;

/// <summary>Applies the set of <see cref="IEntityTypeConfiguration{TEntity}"/> mappings for one database provider.</summary>
internal interface IDbConfigurationsStrategy
{
    ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder);
}
