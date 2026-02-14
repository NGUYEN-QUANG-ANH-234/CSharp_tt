using ngay_3_toi_uu.RecordsLoaders;
using ngay_3_toi_uu.WordCounters;
using ngay_3_toi_uu.Utilities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ngay_3_toi_uu
    
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // --- CẤU HÌNH HỆ THỐNG ---
            Console.OutputEncoding = Encoding.UTF8;
            const int MaxLimit = 2_000_000_000;
            const int MaxQuantityWord = 100;
            const long MaxFileSizeMb = 51_200;

            // --- NHẬP LIỆU VÀ KHỞI TẠO FILE ---
            string baseDir = AppContext.BaseDirectory;
            string filePath = Path.Combine(baseDir, "DummyLogFile.txt");

            Console.Write("Nhập dung lượng file (MB): ");
            var sizeInMb = InputHelper.GetValidLong($"Vui lòng nhập số dòng (1 - {MaxFileSizeMb:N0}): ", 1, MaxFileSizeMb);

            long targetSizeBytes = sizeInMb << 20;

            FileInfo file = new FileInfo(filePath);
            if (!file.Exists || file.Length < targetSizeBytes)
            {
                Console.WriteLine("[ERROR] File chưa tồn tại. Bắt đầu tạo file mới...");
                GenerateLogFile.Generate(filePath, targetSizeBytes);
            }

            Console.WriteLine($"[OK] Đã tồn tại file xấp xỉ ({sizeInMb} MB), bỏ qua bước tạo file.");

            // --- CẤU HÌNH THÔNG SỐ XỬ LÝ ---
            var lineQuantity = InputHelper.GetValidInt($"Vui lòng nhập số dòng (1 - {MaxLimit:N0}): ", 1, MaxLimit);
            Console.WriteLine($"[OK] Hệ thống sẽ xử lý {lineQuantity:N0} dòng.");

            var wordQuantity = InputHelper.GetValidInt($"Nhập số lượng từ xuất hiện nhiều nhất (1 - {MaxQuantityWord:N0}): ", 1, MaxQuantityWord);
            Console.WriteLine($"[OK] Hệ thống sẽ xử lý {wordQuantity:N0} từ.");

            // --- LOAD FILE ---
            int recordLimit = lineQuantity;
            Stopwatch sw = Stopwatch.StartNew();

            // Sync I/O 
            Console.WriteLine($"--- Đang load {recordLimit:N0} records (Sync I/O) ---");
            var syncProcessor = new SyncLoadRecords();
            var syncRecords = syncProcessor.LoadRecords(filePath, recordLimit);
            sw.Stop();
            Console.WriteLine($"[OK] Load Sync hoàn tất: {sw.ElapsedMilliseconds} ms\n");

            sw.Restart();

            // Async I/O 
            Console.WriteLine($"--- Đang load {recordLimit:N0} records (Async I/O) ---");
            var asyncProcessor = new AsyncLoadRecords();
            var asyncRecords = await asyncProcessor.LoadRecords(filePath, recordLimit);
            sw.Stop();
            Console.WriteLine($"[OK] Load Async hoàn tất: {sw.ElapsedMilliseconds} ms\n");        

            sw.Restart();

            // --- ĐẾM TỪ ---
            // Xử lý tuần tự
            Console.WriteLine("\n--- Đang xử lý TUẦN TỰ ---");
            var sequential = new SequentialWordsCounter();
            sw.Restart();
            sequential.Execute(asyncRecords);
            var seqResults = sequential.GetTopWords(wordQuantity);

            sw.Stop();
            AnalyzeLog.PrintResults(seqResults, sw.ElapsedMilliseconds, sequential.GetTotalWordsCount());

            // Xử lý song song
            Console.WriteLine("\n--- Đang xử lý SONG SONG ---");
            var parallel = new ParallelWordsCounter();
            sw.Restart();
            parallel.Execute(asyncRecords);
            var parResults = parallel.GetTopWords(wordQuantity);

            sw.Stop();
            AnalyzeLog.PrintResults(parResults, sw.ElapsedMilliseconds, parallel.GetTotalWordsCount());
        }
    }
}