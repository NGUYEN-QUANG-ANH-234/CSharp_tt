using Asp.Versioning;
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

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Dùng /v1/ trong URL
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // 2. Đăng ký các dịch vụ từ Infrastructure 
        builder.Services.AddInfrastructure(builder.Configuration);

        var app = builder.Build();

        app.UseMiddleware<ExceptionMiddleware>();    // Bắt lỗi toàn cục
        app.UseMiddleware<CorrelationIdMiddleware>(); // Gán ID ngay từ đầu

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDevTools();
        }

        app.UseHttpsRedirection();

        // Benchmark: Thêm một Middleware tự chế để đo Response Time ở đây
        app.UseMiddleware<PerformanceMiddleware>();        

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