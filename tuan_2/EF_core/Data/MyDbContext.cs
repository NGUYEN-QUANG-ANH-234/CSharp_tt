using Microsoft.EntityFrameworkCore;
using EF_core.Models.Entities;
using Microsoft.Extensions.Logging;

namespace EF_core.Data
{
    internal class MyDbContext : DbContext
    {
        // Chuoi ket noi voi CSDL
        private readonly string _connectionString;

        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter(DbLoggerCategory.Query.Name, LogLevel.Information);

            builder.AddConsole();
        });

        public MyDbContext()
        {
            DotNetEnv.Env.Load();
            var _db_server = Environment.GetEnvironmentVariable("DB_SERVER");
            var _db_server_port = Environment.GetEnvironmentVariable("DB_SERVER_PORT");
            var _db_name = Environment.GetEnvironmentVariable("DB_NAME");
            var _db_user = Environment.GetEnvironmentVariable("DB_USER");
            var _db_pwd = Environment.GetEnvironmentVariable("DB_PWD");

            _connectionString = $"Data Source={_db_server},{_db_server_port}; Initial Catalog={_db_name}; User ID={_db_user}; Password={_db_pwd}";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Cau hinh CSDL
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLoggerFactory(loggerFactory);

            // Xac dinh se lam viec voi CSDL cua SqlServer
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
        }
            
        public DbSet<Products> products { set; get; }
    }
}
