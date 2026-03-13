using DemoWebAPI.Data;
using DemoWebAPI.Mappings;
using DemoWebAPI.Repositories.BaseRepositories;
using DemoWebAPI.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace DemoWebAPI.Extensions;

public static class ServiceExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Đăng ký Controllers & Swagger
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(); // Tạo bộ phát sinh tài liệu Swagger
        services.AddOpenApi();

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

        services
            .AddAutoMapper(typeof(CommentProfile).Assembly)
            .AddAutoMapper(typeof(PostProfile).Assembly);

        Type type = typeof(CommentProfile);

        // lấy assembly chứa class
        var assembly = type.Assembly;

        Console.WriteLine("Assembly: " + assembly.FullName);
    }
}
