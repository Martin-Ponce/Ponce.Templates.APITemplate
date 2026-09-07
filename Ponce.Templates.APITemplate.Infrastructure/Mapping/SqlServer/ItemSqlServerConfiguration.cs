using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.Templates.APITemplate.Domain.Entities.Sample;
using static Ponce.Templates.APITemplate.Domain.DomainConstants.Values;
using static Ponce.Templates.APITemplate.Infrastructure.Mapping.ColumnTypes.SqlServerColumnTypes;

namespace Ponce.Templates.APITemplate.Infrastructure.Mapping.SqlServer;

/// <summary>
/// SQL Server mapping for <see cref="Item"/>. Every column has an explicit name, store type and
/// (where it applies) max length - nothing is left for EF Core to infer. Copy this shape for every
/// new entity, one file per provider.
/// </summary>
internal class ItemSqlServerConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType(UNIQUEIDENTIFIER)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(NVARCHAR)
            .HasMaxLength(ItemLimits.MAX_LENGTH_NAME)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType(NVARCHAR)
            .HasMaxLength(ItemLimits.MAX_LENGTH_DESCRIPTION)
            .IsRequired(false);

        builder.Property(x => x.Enabled)
            .HasColumnName("enabled")
            .HasColumnType(BIT)
            .IsRequired();

        builder.Property(x => x.RegisteredAtUtc)
            .HasColumnName("registeredAtUtc")
            .HasColumnType(DATETIME2)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updatedAtUtc")
            .HasColumnType(DATETIME2)
            .IsRequired(false);
    }
}
