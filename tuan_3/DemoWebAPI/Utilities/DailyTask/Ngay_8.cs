using DemoWebAPI.Data;
using DemoWebAPI.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DemoWebAPI.Utilities.DailyTask
{
    public static class Ngay_8
    {         

        // --- DAY 8: DEMO N+1 PROBLEM(LAZY & EXPLICIT) ---
        public static async Task Demo_N_plus_1(MyDbContext dbContext, CommentRepo commentRepo)
        {
            var performanceResults = new List<(string Method, long Time, string Note)>();
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("DAY 8: PHÂN TÍCH VẤN ĐỀ N+1");
            Console.WriteLine(new string('=', 60));

            // Demo Explicit Loading (Nạp thủ công từng Reference)
            var sw = Stopwatch.StartNew();

            var expComments = await dbContext.comments.Take(5).ToListAsync();
            Console.WriteLine("\n> [Explicit Loading] Nạp thủ công Reference(x => x.User):");
            foreach (var c in expComments)
            {
                await dbContext.Entry(c).Reference(x => x.User).LoadAsync();
                Console.WriteLine($"  - {c.Text} (By: {c.User.FName})");
            }
            sw.Stop();
            performanceResults.Add(("Explicit", sw.ElapsedMilliseconds, "1 Query + N query"));

            // Demo Lazy Loading (Trigger query trong vòng lặp)
            sw.Restart();
            var lazyComments = await dbContext.comments.Take(5).ToListAsync();
            Console.WriteLine("> [Lazy Loading] Đang in 5 mẫu (Xem SQL Log để thấy N+1):");
            foreach (var c in lazyComments) Console.WriteLine($"  - {c.Text} (By: {c.User.FName})");
            sw.Stop();
            performanceResults.Add(("Lazy", sw.ElapsedMilliseconds, "1 Query + N query"));


            // Giải quyết bằng Eager Loading
            sw.Restart();
            var eagerComments = await dbContext.comments.Take(5).Include(c => c.User).ToArrayAsync();
            Console.WriteLine("\n> [Eager Loading] Chủ động nạp toàn bộ dữ liệu (có thể dùng):");
            foreach (var c in eagerComments)
            {
                await dbContext.Entry(c).Reference(x => x.User).LoadAsync();
                Console.WriteLine($"  - {c.Text} (By: {c.User.FName})");
            }
            sw.Stop();
            performanceResults.Add(("Eager", sw.ElapsedMilliseconds, "1 Query + Fix-up RAM"));

            // Hiển thị bảng so sánh hiệu năng
            DataVisualizer.DisplayComparisonTable(performanceResults);
        }
    }
}
