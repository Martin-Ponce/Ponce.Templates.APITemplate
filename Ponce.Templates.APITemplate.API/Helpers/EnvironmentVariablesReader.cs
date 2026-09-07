namespace Ponce.Templates.APITemplate.API.Helpers;

public static class EnvironmentVariablesReader
{
    public static string? TryOverwriteWithEnviromentValue(this string? originalValue, string environmentVariableName)
    {
        var environmentValue = Environment.GetEnvironmentVariable(environmentVariableName.ToUpper().Trim());
        if (!string.IsNullOrEmpty(environmentValue))
            return environmentValue;
        return originalValue;
    }

    public static int TryOverwriteWithEnviromentValue(this int originalValue, string environmentVariableName)
    {
        var environmentValue = Environment.GetEnvironmentVariable(environmentVariableName.ToUpper().Trim());
        if (!string.IsNullOrEmpty(environmentValue) && int.TryParse(environmentValue, out var value))
            return value;
        return originalValue;
    }
}
