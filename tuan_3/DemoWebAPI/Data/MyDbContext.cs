using DotNetEnv;
using DemoWebAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;

namespace DemoWebAPI.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        // LoggerFactory nên để static để tránh tạo mới mỗi lần khởi tạo Context
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter(DbLoggerCategory.Query.Name, LogLevel.Information)
                   .SetMinimumLevel(LogLevel.Warning)
                   .AddConsole();
        });


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
