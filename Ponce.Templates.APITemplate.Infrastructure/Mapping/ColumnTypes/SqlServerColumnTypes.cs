namespace Ponce.Templates.APITemplate.Infrastructure.Mapping.ColumnTypes;

/// <summary>
/// SQL Server store type names used by the <c>*SqlServerConfiguration</c> mapping classes.
/// Add entries here as new column types are needed - never let EF Core infer a store type.
/// </summary>
internal static class SqlServerColumnTypes
{
    public const string NVARCHAR = "nvarchar";
    public const string VARCHAR = "varchar";
    public const string BIT = "bit";
    public const string DATETIME2 = "datetime2";
    public const string UNIQUEIDENTIFIER = "uniqueidentifier";
}
