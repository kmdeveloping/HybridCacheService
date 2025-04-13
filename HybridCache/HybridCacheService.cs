using System.Text.Json;
using HybridCache.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

namespace HybridCache;

public class HybridCacheService(HybridCacheConfiguration configuration, IMemoryCache memoryCache, IDistributedCache distributedCache) : IHybridCacheService
{
    private readonly TimeSpan _memoryDuration = configuration.MemoryCacheDuration ?? TimeSpan.FromMinutes(1);
    private readonly TimeSpan _redisDuration = configuration.DistributedCacheDuration ?? TimeSpan.FromMinutes(10);
    private readonly TimeSpan _defaultSlidingExpiration = configuration.DefaultSlidingExpiration ?? TimeSpan.FromSeconds(30);
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly IDistributedCache _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory) where T : class
    {
        if (_memoryCache.TryGetValue(key, out T? value)) return value;
        
        var redisValue = await _distributedCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(redisValue))
        {
            value = JsonSerializer.Deserialize<T>(redisValue);
            if (value != null)
            {
                _memoryCache.Set(key, value, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _memoryDuration,
                    SlidingExpiration = _defaultSlidingExpiration,
                    Priority = CacheItemPriority.Normal
                });
            }
            return value;
        }

        value = await factory();
        if (value == null) return value;
        
        _memoryCache.Set(key, value, _memoryDuration);
        var serialized = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _redisDuration,
            SlidingExpiration = _defaultSlidingExpiration
        });

        return value;
    }
}
