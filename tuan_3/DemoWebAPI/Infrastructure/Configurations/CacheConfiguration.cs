using DemoWebAPI.Infrastructure.ExternalServices;
using DemoWebAPI.Application.Interfaces;

namespace DemoWebAPI.WebAPI.Extensions;

public static class CacheConfiguration
{
    public static IServiceCollection AddCacheConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "DemoWebAPI_";
        });

        services.AddSingleton<IAppCache, RedisAppCache>();
        services.AddSingleton<ILockService, LockService>();

        return services;
    }
}