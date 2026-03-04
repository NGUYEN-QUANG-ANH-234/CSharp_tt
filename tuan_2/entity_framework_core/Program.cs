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

            //// --- DAY 6: SEED DATA ---
            //Console.WriteLine("DAY 6: TẠO DATASET");
            //await MockDataGenerator.SeedAll(dbContext);
            //DataVisualizer.ShowSeedReport(100000, 100000, 100000);

            // --- DAY 8: DEMO N+1 PROBLEM (LAZY & EXPLICIT) ---
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

            // --- DAY 9: SO SÁNH HIỆU NĂNG ---
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("DAY 9: PERFORMANCE TUNING (POST-LEVEL)");
            Console.WriteLine(new string('=', 60));
            
            // 1. Đo Eager Loading
            sw.Restart();
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
            var cteFlatList = await commentRepo.GetAllCommentsCTE(postId);
            sw.Stop();
            performanceResults.Add(("CTE Recursive", sw.ElapsedMilliseconds, "DB-side Recursion"));

            // 5.Đo DeRecursion_EagerLoaing Query
            sw.Restart();
            var deCurs_Eager_FlatList = await commentRepo.DeRecursion_EagerLoading(postId);
            sw.Stop();
            performanceResults.Add(("Eager (DeRecursion)", sw.ElapsedMilliseconds, "De-Recursion"));

            // 4.Đo DeRecursion_LazyLoading Query
            sw.Restart();
            var deCurs_Lazy_FlatList = await commentRepo.DeRecursion_LazyLoading(postId);
            sw.Stop();
            performanceResults.Add(("Lazy (DeRecursion)", sw.ElapsedMilliseconds, "De-Recursion"));

            // Hiển thị bảng so sánh hiệu năng
            DataVisualizer.DisplayComparisonTable(performanceResults);

            // --- HIỂN THỊ CẤU TRÚC CÂY & FLATTEN ---
            Console.WriteLine("\nCAU TRUC CAY COMMENT (TRỰC QUAN):");
            DataVisualizer.VisualizingTree(eagerTree); // Sử dụng kết quả từ Eager Loading

            Console.WriteLine("\nKẾT QUẢ PHÁ ĐỆ QUY (FLATTEN) - EXPLICIT:");
            var explicit_FlatList = commentRepo.FlattenTreeWithAnalysis(explicitList); 
            DataVisualizer.ShowFlattenedResult(explicit_FlatList);

            Console.WriteLine("\nKẾT QUẢ PHÁ ĐỆ QUY (FLATTEN) - CTE:");
            var cte_FlatList = commentRepo.FlattenTreeWithAnalysis(explicitList);
            DataVisualizer.ShowFlattenedResult(cte_FlatList);

            Console.ReadKey();
        }
    }
}