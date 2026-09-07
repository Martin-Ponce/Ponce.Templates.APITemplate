namespace Ponce.Templates.APITemplate.Domain.Helpers;

public static class EnumUtil
{
    public static TEnum ParseOrDefault<TEnum>(this string? value, TEnum defaultValue)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        if (Enum.TryParse<TEnum>(value, true, out var result) &&
                    Enum.IsDefined(result))
        {
            return result;
        }

        return defaultValue;
    }
}
