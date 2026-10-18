namespace Ponce.Templates.APITemplate.Domain;

public static class DomainConstants
{
    public static class ExceptionSources
    {
        public const string APPSETTINGS = "Configuration file";
        public const string DATABASE = "Database";
        public const string CONNECTION = "Connection";
    }

    /// <summary>Error codes grouped by exception type, keep them unique within each group.</summary>
    public static class CustomExceptionCodes
    {
        #region Not Found
        public const int EXAMPLEITEM_NOT_FOUND = 1;
        #endregion

        #region Invalid Operation
        #endregion
    }

    public static class Values
    {
        public const int MIN_LENGTH_VALUE = 3;

        public static class ExampleItemLimits
        {
            public const int MIN_LENGTH_NAME = MIN_LENGTH_VALUE;
            public const int MAX_LENGTH_NAME = 128;
            public const int MAX_LENGTH_DESCRIPTION = 512;
        }
    }
}
