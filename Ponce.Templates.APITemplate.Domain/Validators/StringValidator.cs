using System.Diagnostics.CodeAnalysis;
using Ponce.Templates.APITemplate.Domain.Exceptions;

namespace Ponce.Templates.APITemplate.Domain.Validators;

public static class StringValidator
{
    public static ValidationException ExceptionWhenOutOfRangeArguments(string argumentName, int maxLength) =>
        ValidationException.OutOfRange($"Argument '{argumentName}' must have between 1 and {maxLength} characters");

    public static ValidationException ExceptionWhenNullArgument(string argumentName) =>
        ValidationException.NullOrEmpty($"Argument '{argumentName}' cannot be empty");

    public static string ValidateLength(this string? value, int maxLength, AppException exceptionWhenInvalid, int minLength = 0)
    {
        value.ValidateNull(exceptionWhenInvalid);
        if (value.Length > maxLength || value.Length < minLength)
        {
            throw exceptionWhenInvalid;
        }
        return value;
    }

    public static string ValidateNull([NotNull] this string? value, AppException exceptionWhenInvalid)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw exceptionWhenInvalid;
        }
        return value;
    }

    public static string ValidateRegex(this string? value, string regexPattern, AppException exceptionWhenInvalid)
    {
        if (string.IsNullOrEmpty(value) || !System.Text.RegularExpressions.Regex.IsMatch(value, regexPattern))
        {
            throw exceptionWhenInvalid;
        }
        return value;
    }
}
