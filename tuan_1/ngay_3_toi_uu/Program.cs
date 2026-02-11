using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ngay_3_toi_uu
    
{
    public abstract class WordCounter
    {
        protected static readonly char[] Separators = { ' ', '.', ',', ':', ';', '\t', '(', ')', '{', '}', '-', '_', '+', '=', '/' };

        // PLINQ - Sử dụng Parallel LINQ để lấy Top Words
        public IDictionary<string, long> GetTopWordsPLINQ(IDictionary<string, long> results, int n)
        {
            return results.AsParallel()
                .OrderByDescending(pair => pair.Value)
                .Take(n)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }

    public class LogProcessor : WordCounter
    {
        // Async I/O - Đọc file bất đồng bộ
        public async Task<List<string>> LoadRecordsAsync(string filePath, int limit)
        {
            var records = new List<string>(limit);
            var reader = new StreamReader(filePath);
            string line;
            int count = 0;
            while (count < limit && (line = await reader.ReadLineAsync()) != null)
            {
                records.Add(line);
                count++;
            }
            return records;
        }

        // Xử lý tuần tự
        public Dictionary<string, long> ProcessSequential(List<string> records)
        {
            var dict = new Dictionary<string, long>();
            foreach (var line in records)
            {
                var words = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    if (dict.ContainsKey(word)) dict[word]++;
                    else dict[word] = 1;
                }
            }
            return dict;
        }

        // Xử lý song song bằng Parallel LINQ (Thay thế Parallel.ForEach)
        public IDictionary<string, long> ProcessPLINQ(List<string> records)
        {
            return records.AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .SelectMany(line => line.Split(Separators, StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(word => word)
                .ToDictionary(g => g.Key, g => (long)g.Count());
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // 1. Chuẩn bị dữ liệu
            string filePath = @"D:\hoc_lap_trinh\c_sharp_tt\tuan_1\ngay_1_2\DummyLogFile.txt";

            Console.WriteLine("Nhập số dòng trong Records muốn xử lý: ");
            int recordLimit = Convert.ToInt32(Console.ReadLine()); 
            var processor = new LogProcessor();

            // 2. Day 3: Async I/O - Load 10^6 records vào RAM để Benchmark CPU
            Console.WriteLine($"--- Đang load {recordLimit:N0} records (Async I/O) ---");
            Stopwatch sw = Stopwatch.StartNew();
            var records = await processor.LoadRecordsAsync(filePath, recordLimit);
            sw.Stop();
            Console.WriteLine($"Load hoàn tất: {sw.ElapsedMilliseconds} ms\n");

            // 3. Benchmark Tuần tự
            Console.WriteLine("--- Đang xử lý TUẦN TỰ ---");
            sw.Restart();
            var seqResults = processor.ProcessSequential(records);
            var seqTop = processor.GetTopWordsPLINQ(seqResults, 5);
            sw.Stop();
            Console.WriteLine($"Thời gian Tuần tự: {sw.ElapsedMilliseconds} ms");

            // Hiển thị kết quả kiểm chứng
            Console.WriteLine("\nTop 5 từ khóa:");
            foreach (var item in seqTop)
                Console.WriteLine($"- {item.Key}: {item.Value}");

            // 4. Day 3: Benchmark PLINQ (Song song)
            Console.WriteLine("\n--- Đang xử lý SONG SONG (PLINQ) ---");
            sw.Restart();
            var plinqResults = processor.ProcessPLINQ(records);
            var plinqTop = processor.GetTopWordsPLINQ(plinqResults, 5);
            sw.Stop();
            Console.WriteLine($"Thời gian PLINQ: {sw.ElapsedMilliseconds} ms");

            // Hiển thị kết quả kiểm chứng
            Console.WriteLine("\nTop 5 từ khóa:");
            foreach (var item in plinqTop)
                Console.WriteLine($"- {item.Key}: {item.Value}");
        }
    }
}