using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Application.Mappings;
using DemoWebAPI.Application.Services;
using DemoWebAPI.Core.Interfaces;
using DemoWebAPI.Infrastructure.Data;
using DemoWebAPI.Infrastructure.Persistence.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DemoWebAPI.Infrastructure.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services)
    {
        DotNetEnv.Env.Load();

        var connString = $"Data Source={Environment.GetEnvironmentVariable("DB_SERVER")}, {Environment.GetEnvironmentVariable("DB_PORT")}; " +
                         $"Initial Catalog={Environment.GetEnvironmentVariable("DB_SERVER_NAME")}; " +
                         $"User ID={Environment.GetEnvironmentVariable("DB_USER")}; " +
                         $"Password={Environment.GetEnvironmentVariable("DB_PWD")}";

        services.AddDbContext<MyDbContext>(options =>
            options.UseMySql(connString, ServerVersion.AutoDetect(connString))
                   .LogTo(message => Log.Information(message), new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                   // Benchmark: Cho phép xem chi tiết lỗi và log SQL
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors()
                   .UseLazyLoadingProxies());

        return services;
    }
}
