using DemoWebAPI.Data;
using DemoWebAPI.Extensions;
using System.Text;

namespace DemoWebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var builder = WebApplication.CreateBuilder(args);

            // Đăng ký Infrastructure (đã bao gồm SwaggerGen)
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            // Đảm bảo UseDevTools được gọi ĐẦU TIÊN để cấu hình Swagger
            app.UseDevTools();

            // Các Middleware khác
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
