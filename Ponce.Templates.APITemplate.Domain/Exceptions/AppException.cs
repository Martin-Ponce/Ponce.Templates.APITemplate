namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>
/// Base type for every domain/application exception in this template: a typed exception with a
/// numeric <c>Code</c> and optional <c>Args</c>, with a specific subtype per error category, that
/// <see cref="Middlewares.ErrorHandlerMiddleware"/> maps to an HTTP status code.
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
