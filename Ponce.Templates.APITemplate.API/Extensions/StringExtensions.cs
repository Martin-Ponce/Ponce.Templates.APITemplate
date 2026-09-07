using System.Text;
using Ponce.Templates.APITemplate.Domain.Exceptions;
using Ponce.Templates.APITemplate.Domain.Validators;

namespace Ponce.Templates.APITemplate.API.Extensions;

public static class StringExtensions
{
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var noAccents = new string(value
            .Normalize(NormalizationForm.FormD)
            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray());

        var parts = noAccents.Split([' '], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return string.Empty;

        var resultBuilder = new StringBuilder(parts[0].ToLowerInvariant());
        for (int i = 1; i < parts.Length; i++)
        {
            if (parts[i].Length == 0) continue;
            resultBuilder.Append(char.ToUpperInvariant(parts[i][0]))
                         .Append(parts[i][1..].ToLowerInvariant());
        }
        return resultBuilder.ToString();
    }

    public static string ToUrlWithoutSlash(this string? url, AppException exceptionWhenInvalid)
    {
        url.ValidateNull(exceptionWhenInvalid);
        if (url.EndsWith('/'))
            url = url.TrimEnd('/');
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw exceptionWhenInvalid;
        return url;
    }

    public static bool ToBool(this string? boolean, AppException exceptionWhenInvalid)
    {
        boolean.ValidateNull(exceptionWhenInvalid);
        if (!bool.TryParse(boolean, out var booleanBool))
            throw exceptionWhenInvalid;
        return booleanBool;
    }

    public static string FromPascal(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var result = new StringBuilder();
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (char.IsUpper(c) && i > 0)
            {
                result.Append(' ');
            }
            result.Append(c);
        }
        return result.ToString();
    }
}
