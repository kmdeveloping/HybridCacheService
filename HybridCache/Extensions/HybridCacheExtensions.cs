using HybridCache.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HybridCache.Extensions;

public static class HybridCacheExtensions
{
    public static void AddHybridCacheService(this IServiceCollection services, IOptions<HybridCacheOptions>? options = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (options is null)
        {
            var defaultOptions = new HybridCacheOptions
            {
                RedisDistributedCacheEnabled = false,
                MemoryCacheDuration = TimeSpan.FromMinutes(5),
                DistributedCacheDuration = TimeSpan.FromMinutes(10),
                DefaultSlidingExpiration = TimeSpan.FromSeconds(30)
            };
            
            options = new OptionsWrapper<HybridCacheOptions>(defaultOptions);
        }
        
        services.AddSingleton<HybridCacheOptions>(options.Value);
        
        if (!options.Value.RedisDistributedCacheEnabled)
            services.AddDistributedMemoryCache();
        else
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = options.Value.RedisDistributedCacheConnectionString;
                opt.InstanceName = options.Value.RedisDistributedCacheInstanceName;
            });

        services.AddSingleton<IHybridCacheService, HybridCacheService>();
    }
}