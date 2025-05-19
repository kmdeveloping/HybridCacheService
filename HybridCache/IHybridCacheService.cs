namespace HybridCache;

public interface IHybridCacheService
{
    /// <summary>
    /// async method to get the value from l1 or l2 cache.
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T?> GetAsync<T>(string key) where T : class;
    /// <summary>
    /// method to get the value from l1 or l2 cache.
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? Get<T>(string key) where T : class;
    /// <summary>
    /// async method to set the value to l1 and l2 cache.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task SetAsync<T>(string key, T value) where T : class;
    /// <summary>
    /// method to set the value to l1 and l2 cache.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    void Set<T>(string key, T value) where T : class;
    /// <summary>
    /// async method to get the value from l1 or l2 cache and if not found,
    /// then call the service and set the value to l1 and l2 cache.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="service"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> service) where T : class;
    /// <summary>
    /// method to get the value from l1 or l2 cache and if not found,
    /// then call the service and set the value to l1 and l2 cache.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="service"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? GetOrSet<T>(string key, Func<T> service) where T : class;
    /// <summary>
    /// store exceptions in cache for debugging purpose.
    /// the default ttl is 7 days.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="exception"></param>
    /// <param name="ttl"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task StoreHandledExceptionAsync<T>(string key, T exception, TimeSpan? ttl = null) where T : Exception;
    /// <summary>
    /// fetch the stored exception from the cache.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<string?> GetStoredExceptionAsync(string key);
}