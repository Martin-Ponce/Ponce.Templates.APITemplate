namespace Ponce.Templates.APITemplate.Infrastructure.Mapping.ColumnTypes;

/// <summary>
/// SQLite store type names used by the <c>*SqliteConfiguration</c> mapping classes. SQLite only has
/// a handful of storage classes, but they are still spelled out explicitly (never inferred) so every
/// provider mapping follows the same discipline.
/// </summary>
internal static class SqliteColumnTypes
{
    public const string TEXT = "TEXT";
    public const string INTEGER = "INTEGER";
}
