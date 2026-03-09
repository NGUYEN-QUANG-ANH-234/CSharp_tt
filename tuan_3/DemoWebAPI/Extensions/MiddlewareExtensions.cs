namespace DemoWebAPI.Extensions;
public static class MiddlewareExtensions
{
    public static void UseDevTools(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            // Bật giao diện Swagger UI trong môi trường Development
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}