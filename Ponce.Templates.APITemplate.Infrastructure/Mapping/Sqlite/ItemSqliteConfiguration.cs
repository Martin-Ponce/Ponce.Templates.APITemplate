using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.Templates.APITemplate.Domain.Entities.Sample;
using static Ponce.Templates.APITemplate.Domain.DomainConstants.Values;
using static Ponce.Templates.APITemplate.Infrastructure.Mapping.ColumnTypes.SqliteColumnTypes;

namespace Ponce.Templates.APITemplate.Infrastructure.Mapping.Sqlite;

/// <summary>
/// SQLite mapping for <see cref="Item"/>. Every column has an explicit name, store type and (where
/// it applies) max length - nothing is left for EF Core to infer. Copy this shape for every new
/// entity, one file per provider.
/// </summary>
internal class ItemSqliteConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType(TEXT)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(TEXT)
            .HasMaxLength(ItemLimits.MAX_LENGTH_NAME)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType(TEXT)
            .HasMaxLength(ItemLimits.MAX_LENGTH_DESCRIPTION)
            .IsRequired(false);

        builder.Property(x => x.Enabled)
            .HasColumnName("enabled")
            .HasColumnType(INTEGER)
            .IsRequired();

        builder.Property(x => x.RegisteredAtUtc)
            .HasColumnName("registered_at_utc")
            .HasColumnType(TEXT)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType(TEXT)
            .IsRequired(false);
    }
}
