namespace DemoWebAPI.WebAPI.Extensions;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddOpenApi();
        return services;
    }
}