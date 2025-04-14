using HybridCache.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HybridCache.Extensions;

public static class HybridCacheExtensions
{
    public static void AddHybridCacheService(this IServiceCollection services, Action<HybridCacheOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);
        
        var options = new HybridCacheOptions();
        configureOptions.Invoke(options);
        
        services.AddSingleton<HybridCacheOptions>(options);
        
        if (!options.RedisCacheOptions.RedisDistributedCacheEnabled)
            services.AddDistributedMemoryCache();
        else
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = options.RedisCacheOptions.RedisDistributedCacheConnectionString;
                opt.InstanceName = options.RedisCacheOptions.RedisDistributedCacheInstanceName;
            });

        services.AddSingleton<IHybridCacheService, HybridCacheService>();
    }
}