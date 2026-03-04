using entity_framework_core.Models.Entities;
using entity_framework_core.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace entity_framework_core.Utilities.DailyTask
{
    public static class Ngay_9
    {
        // --- DAY 9: SO SANH HIEU NANG (PERFORMANCE) ---
        public static void DisplayComparisonTable(List<(string Method, long Time, string Note)> results)
        {
            Console.WriteLine("\n" + new string('-', 70));
            Console.WriteLine(string.Format("| {0,-25} | {1,-10} | {2,-25} |", "Phuong phap (Day 9)", "Time (ms)", "Ghi chu"));
            Console.WriteLine(new string('-', 70));

            foreach (var res in results)
            {
                Console.WriteLine(string.Format("| {0,-25} | {1,-10} | {2,-25} |", res.Method, res.Time, res.Note));
            }
            Console.WriteLine(new string('-', 70));

            results.Clear();
        }

        // --- DAY 9: KET QUA PHÁ ĐỆ QUY (FLATTEN) ---
        public static void ShowFlattenedResult(List<Comment> flatList)
        {
            Console.WriteLine("\n[FLATTEN RESULT]");
            Console.WriteLine($"- Tong so ban ghi sau khi pha de quy: {flatList.Count}");

            for (int i = 0; i < flatList.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] ID: {flatList[i].Id.ToString()[..8]}... | Content: {flatList[i].Text}");
            }
        }

        public static async Task PerformanceTuningAsync(CommentRepo commentRepo, Guid postId)
        {
            var performanceResults = new List<(string Method, long Time, string Note)>();

            // --- SO SÁNH HIỆU NĂNG ---
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("DAY 9: PERFORMANCE TUNING (POST-LEVEL)");
            Console.WriteLine(new string('=', 60));

            // 1. Đo Eager Loading
            Stopwatch sw = Stopwatch.StartNew();
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

            // 4.Đo DeRecursion_LazyLoading Query
            sw.Restart();
            var deCurs_Lazy_FlatList = await commentRepo.DeRecursion_LazyLoading(postId);
            sw.Stop();
            performanceResults.Add(("Lazy (DeRecursion)", sw.ElapsedMilliseconds, "De-Recursion"));

            // 5.Đo DeRecursion_EagerLoaing Query
            sw.Restart();
            var deCurs_Eager_FlatList = await commentRepo.DeRecursion_EagerLoading(postId);
            sw.Stop();
            performanceResults.Add(("Eager (DeRecursion)", sw.ElapsedMilliseconds, "De-Recursion"));

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

            Console.WriteLine("\nKẾT QUẢ PHÁ ĐỆ QUY (FLATTEN) - EAGER - DECURSION:");
            var deCursEager_FlatList = commentRepo.FlattenTreeWithAnalysis(deCurs_Eager_FlatList);
            DataVisualizer.ShowFlattenedResult(deCursEager_FlatList);

            Console.WriteLine("\nKẾT QUẢ PHÁ ĐỆ QUY (FLATTEN) - LAZY - DECURSION:");
            var deCursLazy_FlatList = commentRepo.FlattenTreeWithAnalysis(deCurs_Lazy_FlatList);
            DataVisualizer.ShowFlattenedResult(deCursLazy_FlatList);
        }
    }
}
