namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>Operation not allowed in the current state (400).</summary>
public class InvalidOperationAppException : AppException
{
    public InvalidOperationAppException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
        : base(code, message, args) { }
}
