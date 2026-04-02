using DemoWebAPI.Core.Interfaces;
using DemoWebAPI.Infrastructure.Data;
using DemoWebAPI.Infrastructure.Persistence.Implementations;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DemoWebAPI.WebAPI.Extensions;

public static class DbConfiguration
{
    public static IServiceCollection AddDatabaseConfig(this IServiceCollection services)
    {
        DotNetEnv.Env.Load();
        var connString = $"Server={Environment.GetEnvironmentVariable("DB_SERVER")};" +
                         $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                         $"Database={Environment.GetEnvironmentVariable("DB_SERVER_NAME")};" +
                         $"Uid={Environment.GetEnvironmentVariable("DB_USER")};" +
                         $"Pwd={Environment.GetEnvironmentVariable("DB_PWD")};" +
                         "SslMode=None;AllowPublicKeyRetrieval=True;Max Pool Size=1000;";

        services.AddDbContext<MyDbContext>(options =>
            options.UseMySql(connString, ServerVersion.AutoDetect(connString),
                mySqlOptions => { mySqlOptions.EnableRetryOnFailure(); })
                   .LogTo(message => Log.Information(message), new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                   .UseLoggerFactory(MyDbContext.MyLoggerFactory)
                   .EnableDetailedErrors()
                   .UseLazyLoadingProxies());

        // Repositories
        services.AddScoped<ICommentRepo, CommentRepo>();
        services.AddScoped<IPostRepo, PostRepo>();
        services.AddScoped<IUserRepo, UserRepo>();

        return services;
    }
}