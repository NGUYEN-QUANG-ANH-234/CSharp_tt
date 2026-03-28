using DemoWebAPI.Core.Interfaces;
using DemoWebAPI.Infrastructure.Persistence.Implementations;
using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Application.Services;

namespace DemoWebAPI.Infrastructure.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositoriesAndServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ICommentRepo, CommentRepo>();
        services.AddScoped<IPostRepo, PostRepo>();
        services.AddScoped<IUserRepo, UserRepo>();

        // Services
        services.AddScoped<ICommentService, CommentService>();

        // External Services
        services.AddSingleton<IAppCache, RedisAppCache>();

        return services;
    }
}
