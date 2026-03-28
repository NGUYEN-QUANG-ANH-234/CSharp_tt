using Microsoft.Extensions.Configuration;
using Serilog;

namespace DemoWebAPI.Infrastructure.Configurations;

public static class LoggingConfiguration
{
    public static void SetupInfrastructureLogging(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
    }
}