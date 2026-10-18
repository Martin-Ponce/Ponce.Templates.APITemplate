using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.Templates.APITemplate.Domain.Entities.Example;
using static Ponce.Templates.APITemplate.Domain.DomainConstants.Values;
using static Ponce.Templates.APITemplate.Infrastructure.Mapping.ColumnTypes.SqliteColumnTypes;

namespace Ponce.Templates.APITemplate.Infrastructure.Mapping.Sqlite;

/// <summary>SQLite mapping for example items.</summary>
internal class ExampleItemSqliteConfiguration : IEntityTypeConfiguration<ExampleItem>
{
    public void Configure(EntityTypeBuilder<ExampleItem> builder)
    {
        builder.ToTable("ExampleItems")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType(TEXT)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(TEXT)
            .HasMaxLength(ExampleItemLimits.MAX_LENGTH_NAME)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType(TEXT)
            .HasMaxLength(ExampleItemLimits.MAX_LENGTH_DESCRIPTION)
            .IsRequired(false);
    }
}
