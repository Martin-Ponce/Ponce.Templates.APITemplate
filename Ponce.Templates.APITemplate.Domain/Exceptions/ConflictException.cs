namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>Conflicts with the current state of a resource (409).</summary>
public class ConflictException : AppException
{
    public static readonly int CONFLICT_FOUND = 1;

    public static ConflictException ConflictFound(string message = "", IReadOnlyDictionary<string, object>? args = null) =>
        new(CONFLICT_FOUND, string.IsNullOrEmpty(message) ? "A conflict was found" : message, args);

    public ConflictException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
        : base(code, message, args) { }
}
