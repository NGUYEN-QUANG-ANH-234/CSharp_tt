using DotNetEnv;
using entity_framework_core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;

namespace entity_framework_core.Data
{
    public class MyDbContext : DbContext
    {
        private readonly string _connectionString;

        public MyDbContext(){
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
            builder.AddFilter(DbLoggerCategory.Query.Name , LogLevel.Information);
            builder.AddFilter(DbLoggerCategory.Database.Name, LogLevel.Information);

            builder.SetMinimumLevel(LogLevel.Warning);

            builder.AddConsole();
        });
            

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder) 
        { 
            // Dam bao thuc hien duoc cac thiet lap san trong lop cha cua no
            base.OnConfiguring(optionBuilder);


            optionBuilder
                .UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString))
                .UseLoggerFactory(loggerFactory)
                .EnableDetailedErrors()
                .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Self-reference Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            //VD: USER ↔ POST(1–1)
            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Post) // Chuyen List<Post> trong User.cs thanh Post  
            //    .WithOne(p => p.Author)
            //    .HasForeignKey<Post>(p => p.UserId);

            //modelBuilder.Entity<Post>()
            //    .HasIndex(p => p.UserId)
            //    .IsUnique();
        }


        public DbSet<User> users { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<Comment> comments { get; set; }
    }
}
