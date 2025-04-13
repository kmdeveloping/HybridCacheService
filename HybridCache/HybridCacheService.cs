using System.Text.Json;
using HybridCache.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

namespace HybridCache;

public class HybridCacheService(HybridCacheOptions options, IMemoryCache memoryCache, IDistributedCache distributedCache) : IHybridCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly IDistributedCache _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    
    private readonly TimeSpan _memoryDuration = options.MemoryCacheDuration ?? TimeSpan.FromMinutes(1);
    private readonly TimeSpan _redisDuration = options.DistributedCacheDuration ?? TimeSpan.FromMinutes(10);
    private readonly TimeSpan _defaultSlidingExpiration = options.DefaultSlidingExpiration ?? TimeSpan.FromSeconds(30);

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> service) where T : class
    {
        if (_memoryCache.TryGetValue(key, out T? value)) return value;
        
        var redisValue = await _distributedCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(redisValue))
        {
            value = JsonSerializer.Deserialize<T>(redisValue);
            if (value != null) _memoryCache.Set(key, value, GetDefaultMemoryCacheEntryOptions());
            return value;
        }

        value = await service();
        if (value == null) return value;
        
        _memoryCache.Set(key, value, GetDefaultMemoryCacheEntryOptions());
        
        var serialized = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, serialized, GetDefaultDistributedCacheEntryOptions());
        
        return value;
    }

    public T? GetOrSet<T>(string key, Func<T> service) where T : class
    {
        if (_memoryCache.TryGetValue(key, out T? value)) return value;
        
        var redisValue = _distributedCache.GetString(key);
        if (!string.IsNullOrEmpty(redisValue))
        {
            value = JsonSerializer.Deserialize<T>(redisValue);
            if (value != null) _memoryCache.Set(key, value, GetDefaultMemoryCacheEntryOptions());
            return value;
        }
        
        value = service();
        if (value == null) return value;
        
        _memoryCache.Set(key, value, GetDefaultMemoryCacheEntryOptions());
        
        var serialized = JsonSerializer.Serialize(value);
        _distributedCache.SetString(key, serialized, GetDefaultDistributedCacheEntryOptions());
        
        return value;
    }

    private MemoryCacheEntryOptions GetDefaultMemoryCacheEntryOptions() => 
        new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _memoryDuration,
            SlidingExpiration = _defaultSlidingExpiration,
            Priority = CacheItemPriority.Normal
        };

    private DistributedCacheEntryOptions GetDefaultDistributedCacheEntryOptions() =>
        new DistributedCacheEntryOptions 
        {
            AbsoluteExpirationRelativeToNow = _redisDuration,
            SlidingExpiration = _defaultSlidingExpiration
         };
}
