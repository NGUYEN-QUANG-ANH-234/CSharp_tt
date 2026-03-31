using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Application.Mappings;
using DemoWebAPI.Application.Services;
using DemoWebAPI.Core.Interfaces;
using DemoWebAPI.Infrastructure.Data;
using DemoWebAPI.Infrastructure.ExternalServices;
using DemoWebAPI.Infrastructure.Persistence.Implementations;
using Microsoft.EntityFrameworkCore;

namespace DemoWebAPI.WebAPI.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Đăng ký Controllers & Swagger
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(); // Tạo bộ phát sinh tài liệu Swagger
        services.AddOpenApi();

        services.AddStackExchangeRedisCache(options => 
        {
            options.Configuration = configuration.GetConnectionString("Redis"); // Địa chỉ Redis server
            options.InstanceName = "DemoWebAPI_"; // Tiền tố (prefix) cho các Key để tránh đụng độ
        });

        // Đăng ký qua Configuration
        // var connString = configuration.GetConnectionString("Default");

        // Đăng ký Database & Repositories
        DotNetEnv.Env.Load();
        var connString = 
            $"Data Source={Environment.GetEnvironmentVariable("DB_SERVER")}, {Environment.GetEnvironmentVariable("DB_PORT")}; " +
            $"Initial Catalog={Environment.GetEnvironmentVariable("DB_SERVER_NAME")}; " +
            $"User ID={Environment.GetEnvironmentVariable("DB_USER")}; " +
            $"Password={Environment.GetEnvironmentVariable("DB_PWD")}";

        // Đăng ký DB chuẩn Web API
        services.AddDbContext<MyDbContext>(options =>
            options.UseMySql(connString, ServerVersion.AutoDetect(connString))
                   .UseLoggerFactory(MyDbContext.MyLoggerFactory) // Gắn log từ class Context
                   .EnableDetailedErrors()
                   .UseLazyLoadingProxies());

        services.AddScoped<ICommentRepo, CommentRepo>();
        services.AddScoped<IPostRepo, PostRepo>();
        services.AddScoped<IUserRepo, UserRepo>();

        services.AddSingleton<IAppCache, RedisAppCache>();
        services.AddScoped<ICommentService, CommentService>();

        // Đăng ký JwtService
        services.AddScoped<IJwtService, JwtService>();
        services.AddJwtAuthentication(configuration);

        services.AddAuthorization(options =>
        {
            // Bạn có thể định nghĩa các Policy (Chính sách) nếu muốn chuyên nghiệp hơn
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User", "Admin"));
        });

        services.AddControllers();

        services
            .AddAutoMapper(typeof(CommentProfile).Assembly)
            .AddAutoMapper(typeof(PostProfile).Assembly);

        Type type = typeof(CommentProfile);

        // lấy assembly chứa class
        var assembly = type.Assembly;

        Console.WriteLine("Assembly: " + assembly.FullName);
    }
}
