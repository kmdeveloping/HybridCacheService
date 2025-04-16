# HybridCache

A .NET 9 hybrid caching library combining in-memory and Redis caching with support for primary constructors and minimal boilerplate.

## Install
```bash
dotnet add package HybridCache
```

## Usage
```csharp
builder.Services.AddHybridCache(opt => 
{
    opt.RedisCacheOptions = new RedisCacheOptions 
    {
        RedisDistributedCacheEnabled = true;
        RedisDistributedCacheConnectionString = builder.Configuration.GetConnectionString("Redis_Connection_String_Key_Name") ?? throw new ArgumentNullException(nameof(builder.Configuration), "Redis Connection Uri is missing");
        RedisDistributedCacheInstanceName = builder.Configuration["Redis:ConfigSection:InstanceName"] ?? throw new ArgumentNullException(nameof(builder.Configuration), "Redis InstanceName is missing");
    };
    
    opt.MemoryCacheDuration = TimeSpan.FromMinutes(5);
    opt.DefaultMemorySlidingExpiration = TimeSpan.FromMinutes(1);
    
    opt.DistributedCacheDuration = TimeSpan.FromHours(2);
    opt.DefaultDistributedSlidingExpiration = TimeSpan.FromMinutes(30);
});
```

```csharp
public class SomeJobClass(IHybridCacheService cache, ISomeOtherService service)
{
    private readonly IHybridCacheService _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ISomeOtherService _service = service ?? throw new ArgumentNullException(nameof(service));
    
    // Async version
    public async Task<SomeJobResponse> DoSomeJobMethodAsync() => 
        // provide cache key and the factory function to execute when cache is expired or null
        await _cache.GetOrSetAsync<SomeJobResponse>("_cache_key_name", async () => await _service.GetJobStatusResponseAsync()) ?? new SomeJobResponse();
    
    // non Async version
    public SomeJobResponse DoSomeJobMethod() =>
        _cache.GetOrSet<SomeJobResponse>("_cache_key_name", () => _service.GetJobStatusResponse()) ?? new SomeJobResponse();
}
```
