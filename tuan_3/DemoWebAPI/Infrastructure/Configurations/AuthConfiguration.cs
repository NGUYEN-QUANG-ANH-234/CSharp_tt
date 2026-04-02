using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Application.Services;
using DemoWebAPI.Infrastructure.ExternalServices;

namespace DemoWebAPI.WebAPI.Extensions;

public static class AuthConfiguration
{
    public static IServiceCollection AddSecurityConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtService, JwtService>();

        // Giả định phương thức AddJwtAuthentication của bạn đã được viết sẵn ở đâu đó
        services.AddJwtAuthentication(configuration);

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User", "Admin"));
        });

        return services;
    }
}