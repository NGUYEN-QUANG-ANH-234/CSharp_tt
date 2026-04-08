using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Application.Mappings;
using DemoWebAPI.Application.Services;
using DemoWebAPI.Core.Interfaces;
using DemoWebAPI.Infrastructure.Data;
using DemoWebAPI.Infrastructure.ExternalServices;
using DemoWebAPI.Infrastructure.Persistence.Implementations;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DemoWebAPI.WebAPI.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        // Gọi các Extension Methods đã chia nhỏ
        services.AddSwaggerConfig()
                .AddDatabaseConfig()
                .AddCacheConfig(configuration)
                .AddSecurityConfig(configuration)
                .AddApplicationServices();
    }
}