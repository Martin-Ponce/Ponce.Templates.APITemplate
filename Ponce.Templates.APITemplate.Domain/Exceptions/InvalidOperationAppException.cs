namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>The operation is not allowed given the current state of the system. Mapped to HTTP 400.</summary>
public class InvalidOperationAppException : AppException
{
    public InvalidOperationAppException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
        : base(code, message, args) { }
}
