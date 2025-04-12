using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

namespace HybridCache;

public class HybridCacheService(
    IMemoryCache memoryCache,
    IDistributedCache distributedCache,
    TimeSpan? memoryDuration = null,
    TimeSpan? redisDuration = null
) : IHybridCacheService
{
    private readonly TimeSpan _memoryDuration = memoryDuration ?? TimeSpan.FromMinutes(1);
    private readonly TimeSpan _redisDuration = redisDuration ?? TimeSpan.FromMinutes(10);

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory) where T : class
    {
        if (memoryCache.TryGetValue(key, out T? value))
        {
            return value;
        }

        var redisValue = await distributedCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(redisValue))
        {
            value = JsonSerializer.Deserialize<T>(redisValue);
            if (value != null)
            {
                memoryCache.Set(key, value, _memoryDuration);
            }
            return value;
        }

        value = await factory();
        if (value != null)
        {
            memoryCache.Set(key, value, _memoryDuration);
            var serialized = JsonSerializer.Serialize(value);
            await distributedCache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _redisDuration
            });
        }

        return value;
    }
}
