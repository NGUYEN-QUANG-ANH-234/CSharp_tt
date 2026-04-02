using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Application.Services;
using DemoWebAPI.Application.Mappings;

namespace DemoWebAPI.WebAPI.Extensions;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICommentService, CommentService>();

        services.AddAutoMapper(typeof(CommentProfile).Assembly, typeof(PostProfile).Assembly);

        // Giữ nguyên logic in thông tin Assembly của bạn
        var assembly = typeof(CommentProfile).Assembly;
        Console.WriteLine("Assembly: " + assembly.FullName);

        return services;
    }
}