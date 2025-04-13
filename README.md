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
    opt.RedisDistributedCacheEnabled = true;
    opt.RedisDistributedCacheConnectionString = builder.Configuration.GetConnectionString("Redis_Connection_String_Key_Name") ?? throw new ArgumentNullException(nameof(builder.Configuration), "Redis Connection Uri is missing");
    opt.RedisDistributedCacheInstanceName = builder.Configuration["Redis:ConfigSection:InstanceName"] ?? throw new ArgumentNullException(nameof(builder.Configuration), "Redis InstanceName is missing");
    opt.MemoryCacheDuration = TimeSpan.FromMinutes(2);
    opt.DistributedCacheDuration = TimeSpan.FromMinutes(10);
    opt.DefaultSlidingExpiration = TimeSpan.FromSeconds(30);
});
```

```csharp
public class SomeJobClass(IHybridCacheService cache, ISomeOtherService service)
{
    private readonly IHybridCacheService _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ISomeOtherService _service = service ?? throw new ArgumentNullException(nameof(service));
    
    public async Task<SomeJobResponse> DoSomeJobMethod() => 
        // provide cache key and the factory function to execute when cache is expired or null
        await _cache.GetOrSetAsync<SomeJobResponse>("_cache_key_name", async () => await _service.GetJobStatusResponse());
}
```
