using Serilog;
using Microsoft.Extensions.Logging;

namespace ngay_5.Utilities
{
    public static class LoggingConfigurator
    {
        public static void SetupSeriLog() {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("Logs/system_log.txt", 
                            rollingInterval: RollingInterval.Day, 
                            retainedFileCountLimit: 7)
                .CreateLogger();
        }

        public static ILoggerFactory CreateFactory() 
        {
            return LoggerFactory.Create(builder =>
                {
                    builder.AddSerilog();
                }
            );
        }


    }
}
