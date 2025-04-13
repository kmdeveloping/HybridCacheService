namespace HybridCache.Configuration;

public class HybridCacheOptions
{
    public bool RedisDistributedCacheEnabled { get; set; }
    public TimeSpan? MemoryCacheDuration { get; set; }
    public TimeSpan? DistributedCacheDuration { get; set; }
    public TimeSpan? DefaultSlidingExpiration { get; set; }
    public string? RedisDistributedCacheConnectionString { get; set; }
    public string? RedisDistributedCacheInstanceName { get; set; } 
}