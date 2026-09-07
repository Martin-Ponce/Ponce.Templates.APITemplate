namespace Ponce.Templates.APITemplate.Infrastructure.Mapping.ColumnTypes;

/// <summary>
/// PostgreSQL store type names used by the <c>*PostgresConfiguration</c> mapping classes.
/// Add entries here as new column types are needed - never let EF Core infer a store type.
/// </summary>
internal static class PostgresColumnTypes
{
    public const string VARCHAR = "varchar";
    public const string BOOLEAN = "boolean";
    public const string TIMESTAMPTZ = "timestamptz";
    public const string UUID = "uuid";
}
