using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.Templates.APITemplate.Domain.Entities.Example;
using static Ponce.Templates.APITemplate.Domain.DomainConstants.Values;
using static Ponce.Templates.APITemplate.Infrastructure.Mapping.ColumnTypes.PostgresColumnTypes;

namespace Ponce.Templates.APITemplate.Infrastructure.Mapping.Postgres;

/// <summary>
/// PostgreSQL mapping for <see cref="ExampleItem"/>. Every column has an explicit name, store type
/// and (where it applies) max length - nothing is left for EF Core to infer.
/// </summary>
internal class ExampleItemPostgresConfiguration : IEntityTypeConfiguration<ExampleItem>
{
    public void Configure(EntityTypeBuilder<ExampleItem> builder)
    {
        builder.ToTable("ExampleItems")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType(UUID)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(VARCHAR)
            .HasMaxLength(ExampleItemLimits.MAX_LENGTH_NAME)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType(VARCHAR)
            .HasMaxLength(ExampleItemLimits.MAX_LENGTH_DESCRIPTION)
            .IsRequired(false);
    }
}
