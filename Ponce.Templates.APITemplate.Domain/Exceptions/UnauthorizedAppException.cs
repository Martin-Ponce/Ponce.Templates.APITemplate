namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>Not authenticated (401), or lacking permissions when built via InsufficientPermissions (403).</summary>
public class UnauthorizedAppException : AppException
{
    public static readonly int NOT_AUTHENTICATED = 1;
    public static readonly int INSUFFICIENT_PERMISSIONS = 2;

    public static UnauthorizedAppException InsufficientPermissions(string message = "") =>
        new(INSUFFICIENT_PERMISSIONS, string.IsNullOrEmpty(message) ? "Insufficient permissions" : message);

    public UnauthorizedAppException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
        : base(code, message, args) { }
}
