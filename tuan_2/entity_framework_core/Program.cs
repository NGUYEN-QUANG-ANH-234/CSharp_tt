using entity_framework_core.Data;
using entity_framework_core.Repositories.Implementations;
using entity_framework_core.Utilities;
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
            var performanceResults = new List<(string Method, long Time, string Note)>();

            // --- DAY 6 & 7: SEED DATA ---
            //Console.WriteLine("DAY 6-7: TẠO DATASET");
            //await MockDataGenerator.SeedAll(dbContext);
            //DataVisualizer.ShowSeedReport(100000, 100000, 100000);

            // --- DAY 8: DEMO N+1 PROBLEM (LAZY & EXPLICIT) ---
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("DAY 8: PHÂN TÍCH VẤN ĐỀ N+1");
            Console.WriteLine(new string('=', 60));

            // Demo Lazy Loading (Trigger query trong vòng lặp)
            var lazyComments = await dbContext.comments.Take(5).ToListAsync();
            Console.WriteLine("> [Lazy Loading] Đang in 5 mẫu (Xem SQL Log để thấy N+1):");
            foreach (var c in lazyComments) Console.WriteLine($"  - {c.Text} (By: {c.User.FName})");

            // Demo Explicit Loading (Nạp thủ công từng Reference)
            var expComments = await dbContext.comments.Take(5).ToListAsync();
            Console.WriteLine("\n> [Explicit Loading] Nạp thủ công Reference(x => x.User):");
            foreach (var c in expComments)
            {
                await dbContext.Entry(c).Reference(x => x.User).LoadAsync();
                Console.WriteLine($"  - {c.Text} (By: {c.User.FName})");
            }

            // --- DAY 9: SO SÁNH HIỆU NĂNG ---
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("DAY 9: PERFORMANCE TUNING (POST-LEVEL)");
            Console.WriteLine(new string('=', 60));

            // 1. Đo Eager Loading
            var sw = Stopwatch.StartNew();
            var eagerTree = await commentRepo.GetAllCommentsForPost_EagerLoading(postId);
            sw.Stop();
            performanceResults.Add(("Eager (Include)", sw.ElapsedMilliseconds, "1 Query + Fix-up RAM"));

            // 2. Đo Explicit Loading
            sw.Restart();
            var explicitList = await commentRepo.GetAllCommentsForPost_ExplicitLoading(postId);
            sw.Stop();
            performanceResults.Add(("Explicit (Loop)", sw.ElapsedMilliseconds, "N+1 Queries (Slow)"));

            // 3. Đo CTE Query
            sw.Restart();
            var cteFlat = await commentRepo.GetAllCommentsCTE(postId);
            sw.Stop();
            performanceResults.Add(("CTE Recursive", sw.ElapsedMilliseconds, "DB-side Recursion"));

            // Hiển thị bảng so sánh hiệu năng
            DataVisualizer.DisplayComparisonTable(performanceResults);

            // --- HIỂN THỊ CẤU TRÚC CÂY & FLATTEN ---
            Console.WriteLine("\nCAU TRUC CAY COMMENT (TRỰC QUAN):");
            DataVisualizer.VisualizingTree(eagerTree); // Sử dụng kết quả từ Eager Loading

            Console.WriteLine("\nKẾT QUẢ PHÁ ĐỆ QUY (FLATTEN):");
            var flatList = commentRepo.FlattenTreeWithAnalysis(eagerTree); 
            DataVisualizer.ShowFlattenedResult(flatList); 

            Console.ReadKey();
        }
    }
}