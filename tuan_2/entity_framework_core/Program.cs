using entity_framework_core.Data;
using entity_framework_core.Repositories.Implementations;
using entity_framework_core.Utilities;
using entity_framework_core.Utilities.DailyTask;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;

namespace entity_framework_core
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            using var dbContext = new MyDbContext();
            var commentRepo = new CommentRepo(dbContext);
            var postId = Guid.Parse("08de7923-12ef-44d3-88c9-83decc8fd97f");

            // TODO: Khi thực hiện các bài Day 6, cần comment các dòng cấu hình Logging trong MyDbContext.cs
            // Lý do: Giảm nhiễu Console và tối ưu tốc độ khi xử lý tập dữ liệu lớn (Seed Data).
            /*
                builder.AddFilter(DbLoggerCategory.Query.Name , LogLevel.Information);
                builder.AddFilter(DbLoggerCategory.Database.Name, LogLevel.Information);
            */

            //// --- DAY 6: SEED DATA ---
            //await Ngay_6.SeedData(dbContext);

            // --- DAY 8: DEMO N+1 PROBLEM (LAZY & EXPLICIT) ---
            await Ngay_8.Demo_N_plus_1(dbContext, commentRepo);

            // --- DAY 9: SO SANH HIEU NANG (PERFORMANCE) ---
            await Ngay_9.PerformanceTuningAsync(commentRepo, postId);

            Console.ReadKey();
        }
    }
}