namespace Ponce.Templates.APITemplate.Domain;

public static class DomainConstants
{
    /// <summary>Human-readable sources used when reporting <see cref="Exceptions.ConfigurationException"/>.</summary>
    public static class ExceptionSources
    {
        public const string APPSETTINGS = "Configuration file";
        public const string DATABASE = "Database";
        public const string CONNECTION = "Connection";
    }

    /// <summary>
    /// Numeric error codes grouped by exception type. Keep codes unique within each group; they are
    /// surfaced to API clients via <see cref="Exceptions.AppException.Code"/> so they can be matched
    /// against documentation/localized message tables.
    /// </summary>
    public static class CustomExceptionCodes
    {
        #region Not Found
        public const int ITEM_NOT_FOUND = 1;
        #endregion

        #region Invalid Operation
        public const int ITEM_ALREADY_EXISTS = 1;
        #endregion
    }

    public static class Values
    {
        public const int MIN_LENGTH_VALUE = 3;

        public static class ItemLimits
        {
            public const int MIN_LENGTH_NAME = MIN_LENGTH_VALUE;
            public const int MAX_LENGTH_NAME = 128;
            public const int MAX_LENGTH_DESCRIPTION = 512;
        }
    }
}
