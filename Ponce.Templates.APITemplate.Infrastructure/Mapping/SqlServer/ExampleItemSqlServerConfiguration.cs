using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.Templates.APITemplate.Domain.Entities.Example;
using static Ponce.Templates.APITemplate.Domain.DomainConstants.Values;
using static Ponce.Templates.APITemplate.Infrastructure.Mapping.ColumnTypes.SqlServerColumnTypes;

namespace Ponce.Templates.APITemplate.Infrastructure.Mapping.SqlServer;

/// <summary>SQL Server mapping for example items.</summary>
internal class ExampleItemSqlServerConfiguration : IEntityTypeConfiguration<ExampleItem>
{
    public void Configure(EntityTypeBuilder<ExampleItem> builder)
    {
        builder.ToTable("ExampleItems")
            .HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType(UNIQUEIDENTIFIER)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType(NVARCHAR)
            .HasMaxLength(ExampleItemLimits.MAX_LENGTH_NAME)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType(NVARCHAR)
            .HasMaxLength(ExampleItemLimits.MAX_LENGTH_DESCRIPTION)
            .IsRequired(false);
    }
}
