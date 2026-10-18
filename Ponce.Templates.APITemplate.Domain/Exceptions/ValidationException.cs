namespace Ponce.Templates.APITemplate.Domain.Exceptions;

/// <summary>Invalid input (400).</summary>
public class ValidationException : AppException
{
    public static readonly int INVALID_FORMAT = 1;
    public static readonly int NULL_OR_EMPTY = 2;
    public static readonly int OUT_OF_RANGE = 3;
    public static readonly int NOT_SUPPORTED = 4;

    public static ValidationException InvalidFormat(string message = "") =>
        new(INVALID_FORMAT, string.IsNullOrEmpty(message) ? "Invalid format" : message);
    public static ValidationException NullOrEmpty(string message) => new(NULL_OR_EMPTY, message);
    public static ValidationException OutOfRange(string message) => new(OUT_OF_RANGE, message);
    public static ValidationException NotSupported(string message) => new(NOT_SUPPORTED, message);

    public ValidationException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
        : base(code, message, args) { }
}
