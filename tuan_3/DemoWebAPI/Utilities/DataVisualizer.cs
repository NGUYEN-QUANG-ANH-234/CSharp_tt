using DemoWebAPI.Data;
using DemoWebAPI.Models.Entities;
using DemoWebAPI.Repositories.BaseRepositories;
using DemoWebAPI.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace DemoWebAPI.Utilities
{
    public class DataVisualizer
    {
        // --- DAY 6 & 7: THONG KE DU LIEU ---
        public static void ShowSeedReport(int userCount, int postCount, int commentCount)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine(" KET QUA SEED DATA (DAY 6 & 7)");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine($" Users    : {userCount:N0}");
            Console.WriteLine($" Posts    : {postCount:N0}");
            Console.WriteLine($" Comments : {commentCount:N0}");
            Console.WriteLine(new string('=', 40));
        }

        // --- DAY 8 & 9: HIEN THI CAY COMMENT (TUONG MINH) ---
        public static void VisualizingTree(List<Comment> comments, int level = 0)
        {
            foreach (var c in comments)
            {
                // Su dung ky tu gach noi de phan cap
                string prefix = level == 0 ? "> " : new string(' ', level * 4) + "|-- ";

                Console.WriteLine($"{prefix}[ID:{c.Id.ToString()[..4]}] {c.Text} (By: {c.User?.FName})");

                if (c.Replies != null && c.Replies.Any())
                {
                    VisualizingTree(c.Replies, level + 1); // De quy vo han cap
                }
            }
        }

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
    }
}