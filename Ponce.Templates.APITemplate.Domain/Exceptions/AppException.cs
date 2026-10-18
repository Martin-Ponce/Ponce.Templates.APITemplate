namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>Base exception with a code and optional args, the error middleware maps each subtype to a status code.</summary>
public abstract class AppException : Exception
{
    public int Code { get; }
    public IReadOnlyDictionary<string, object>? Args { get; }

    protected AppException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
        : base(message)
    {
        Code = code;
        Args = args;
    }
}
