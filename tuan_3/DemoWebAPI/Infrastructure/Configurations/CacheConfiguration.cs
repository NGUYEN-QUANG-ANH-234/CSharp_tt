using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoWebAPI.Infrastructure.Configurations;

public static class CacheConfiguration
{
    public static IServiceCollection AddCacheConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "DemoWebAPI_";
        });

        return services;
    }
}
