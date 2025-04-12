namespace HybridCache;

public interface IHybridCacheService
{
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory) where T : class;
}
