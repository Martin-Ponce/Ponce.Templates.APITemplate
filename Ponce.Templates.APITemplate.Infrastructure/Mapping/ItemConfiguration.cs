using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.Templates.APITemplate.Domain.Entities.Sample;
using static Ponce.Templates.APITemplate.Domain.DomainConstants.Values;

namespace Ponce.Templates.APITemplate.Infrastructure.Mapping;

/// <summary>
/// Sample EF Core mapping. Kept provider-agnostic (no explicit HasColumnType calls) so it works
/// unchanged against SqlServer/Postgres/Sqlite; add provider-specific column types only if you
/// need to fine-tune storage for a specific database engine.
/// </summary>
internal class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items")
            .HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(ItemLimits.MAX_LENGTH_NAME)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(ItemLimits.MAX_LENGTH_DESCRIPTION)
            .IsRequired(false);

        builder.Property(x => x.Enabled)
            .IsRequired();

        builder.Property(x => x.RegisteredAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired(false);
    }
}
