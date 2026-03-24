namespace DemoWebAPI.WebAPI.Extensions;
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

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo Web API v1");
            options.RoutePrefix = "swagger"; // Đường dẫn sẽ là localhost:PORT/swagger
        });

        //app.UseSwaggerUI();
        }
    }
}