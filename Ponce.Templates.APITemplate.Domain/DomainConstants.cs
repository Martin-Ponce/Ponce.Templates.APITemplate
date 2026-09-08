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
        public const int EXAMPLEITEM_NOT_FOUND = 1;
        // Add one <ENTITY>_NOT_FOUND = <n> constant per aggregate root that can be missing.
        #endregion

        #region Invalid Operation
        // Add one <ENTITY>_ALREADY_EXISTS = <n> constant per aggregate root with a uniqueness invariant.
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
        // Add one nested <Entity>Limits class per entity with length-bound properties.
    }
}
