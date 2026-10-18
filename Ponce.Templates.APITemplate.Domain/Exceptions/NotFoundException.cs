namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>Resource doesn't exist (404).</summary>
public class NotFoundException : AppException
{
    public NotFoundException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
        : base(code, message, args) { }
}
