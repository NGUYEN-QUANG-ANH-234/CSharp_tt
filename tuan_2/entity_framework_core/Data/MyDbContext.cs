using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using entity_framework_core.Models.Entities;

namespace entity_framework_core.Data
{
    public class MyDbContext : DbContext
    {
        private readonly string _connectionString;

        public MyDbContext() {
            DotNetEnv.Env.Load();
            var _dbServer = Environment.GetEnvironmentVariable("DB_SERVER");
            var _dbServerName = Environment.GetEnvironmentVariable("DB_SERVER_NAME");
            var _dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var _dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var _dbPwd = Environment.GetEnvironmentVariable("DB_PWD");

            _connectionString = $"Data Source={_dbServer}, {_dbPort}; Initial Catalog={_dbServerName}; User ID={_dbUser}; Password={_dbPwd}";
        }

        public readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder => 
        {
            //builder.AddFilter(DbLoggerCategory.Query.Name , LogLevel.Information);

            builder.SetMinimumLevel(LogLevel.Warning);

            builder.AddConsole();
        });
            

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder) 
        { 
            // Dam bao thuc hien duoc cac thiet lap san trong lop cha cua no
            base.OnConfiguring(optionBuilder);

            optionBuilder.UseLoggerFactory(loggerFactory);
            optionBuilder.UseMySql( _connectionString, ServerVersion.AutoDetect(_connectionString)); 
        }

        public DbSet<User> users { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<Comment> comments { get; set; }
    }
}
