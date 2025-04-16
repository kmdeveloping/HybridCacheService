namespace HybridCache;

public interface IHybridCacheService
{
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> service) where T : class;
    T? GetOrSet<T>(string key, Func<T> service) where T : class;
    Task StoreHandledException<T>(string key, T exception, TimeSpan? ttl = null) where T : Exception;
}