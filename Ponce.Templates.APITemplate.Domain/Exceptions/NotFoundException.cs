namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>Requested resource does not exist. Mapped to HTTP 404.</summary>
public class NotFoundException : AppException
{
    public NotFoundException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
        : base(code, message, args) { }
}
