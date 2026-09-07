namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>
/// Base type for every domain/application exception in this template.
/// <para>
/// The original project used the company's internal "Bisoft.Exceptions" NuGet package for this
/// (typed exceptions with numeric codes, a per-component prefix and localizable message templates).
/// That package is not available outside the company, so this is a small, self-contained
/// stand-in with the same shape (Code/Args + a specific subtype per error category) that
/// <see cref="Middlewares.ErrorHandlerMiddleware"/>-equivalent code can map to HTTP status codes.
/// Replace this with the real package if/when it is available in your environment.
/// </para>
/// </summary>
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
