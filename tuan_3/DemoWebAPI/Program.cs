using DemoWebAPI.Infrastructure.Configurations; // Namespace mới từ Infrastructure
using DemoWebAPI.WebAPI.Extensions;             // Namespace cho Middleware
using DemoWebAPI.WebAPI.Middlewares;
using Serilog;
using System.Text;

namespace DemoWebAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var builder = WebApplication.CreateBuilder(args);

        // 1. Cấu hình Logging (Infrastructure)
        LoggingConfiguration.SetupInfrastructureLogging(builder.Configuration);
        builder.Host.UseSerilog();

        // 2. Đăng ký các dịch vụ từ Infrastructure 
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddRepositoriesAndServices();
        
        // 3. Đăng ký các dịch vụ thuộc tầng Presentation (WebAPI)
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        // Đăng ký AutoMapper từ Assembly của Application
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        var app = builder.Build();

        // 4. Cấu hình Middleware (Pipeline)
        // Lưu ý: Luôn để DevTools lên đầu để Swagger bắt được các request
        app.UseDevTools();

        app.UseHttpsRedirection();

        // Benchmark: Thêm một Middleware tự chế để đo Response Time ở đây
        app.UseMiddleware<PerformanceMiddleware>();

        // THỨ TỰ RẤT QUAN TRỌNG
        app.UseMiddleware<ExceptionMiddleware>(); // Bắt lỗi đầu tiên

        app.UseAuthentication(); // "Bạn là ai?"
        app.UseAuthorization();  // "Bạn có được phép vào không?"

        app.MapControllers();

        try
        {
            Log.Information("Starting Web Host...");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}