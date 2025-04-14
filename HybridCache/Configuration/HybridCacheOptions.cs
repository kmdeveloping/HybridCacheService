namespace HybridCache.Configuration;

public class HybridCacheOptions
{
    public TimeSpan? MemoryCacheDuration { get; set; }
    public TimeSpan? DistributedCacheDuration { get; set; }
    public TimeSpan? DefaultDistributedSlidingExpiration { get; set; }
    public TimeSpan? DefaultMemorySlidingExpiration { get; set; }
    public RedisCacheOptions RedisCacheOptions { get; set; } = null!;
}

public class RedisCacheOptions
{
    public bool RedisDistributedCacheEnabled { get; set; }
    public string? RedisDistributedCacheConnectionString { get; set; }
    public string? RedisDistributedCacheInstanceName { get; set; } 
}