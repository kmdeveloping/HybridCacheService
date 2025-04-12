# HybridCache

A .NET 8 hybrid caching library combining in-memory and Redis caching with support for primary constructors and minimal boilerplate.

## Install
```bash
dotnet add package HybridCache
```

## Usage
```csharp
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(opt => opt.Configuration = "localhost:6379");
builder.Services.AddScoped<IHybridCacheService, HybridCacheService>();
```

```csharp
var data = await _cache.GetOrSetAsync("my-key", async () => await GetDataAsync());
```
