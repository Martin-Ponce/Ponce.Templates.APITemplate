using Microsoft.EntityFrameworkCore;

namespace Ponce.Templates.APITemplate.Infrastructure.Helpers.Strategies;

/// <summary>Applies all entity mappings for one provider.</summary>
internal interface IDbConfigurationsStrategy
{
    ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder);
}
