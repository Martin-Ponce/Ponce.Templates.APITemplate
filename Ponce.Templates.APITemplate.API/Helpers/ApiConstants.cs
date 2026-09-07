using Asp.Versioning;
using System.Reflection;

namespace Ponce.Templates.APITemplate.API.Helpers;

public static class ApiConstants
{
    private static readonly ApiVersion _version1 = new(1, 0);
    public static ApiVersion VERSION_1 => _version1;

    public static class ResponseFormats
    {
        public const string EXTENDED_MULTI_RESULT = "extendedmultiresult";
        public const string SIMPLE_MULTI_RESULT = "simplemultiresult";
    }

    // TODO: rename to your API's name - used in Swagger title, the welcome page and the Serilog table name.
    public const string APP_NAME = "API Template";
    public static string LOGGER_TABLE_NAME => $"{APP_NAME}Logs";
    public const string ALLOW_ALL_CORS_POLICY = "AllowAll";
    public const string FIXED_RATE_LIMITING_POLICY = "Fixed";
    public const int MAX_PAGE_SIZE = 100;
    public const int MIN_PAGE_SIZE = 1;
    public static string ASSEMBLY_VERSION => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "";
}
