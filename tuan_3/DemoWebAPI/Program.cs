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

            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            app.UseDevTools();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
