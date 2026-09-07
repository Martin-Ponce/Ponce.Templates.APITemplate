namespace Ponce.Templates.APITemplate.Infrastructure.Configurations;

public class CacheConfiguration
{
    public bool CacheEnabled { get; set; }
    public TimeSpan CacheSlidingDuration { get; set; }
    public TimeSpan CacheAbsoluteDuration { get; set; }
}
