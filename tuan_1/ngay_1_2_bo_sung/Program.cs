using System;
using System.IO;
using System.Text;
using System.Diagnostics;

using ngay_1_2_bo_sung.Engines;
using ngay_1_2_bo_sung.Utilities;

namespace ngay_1_2_bo_sung
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // --- CẤU HÌNH HỆ THỐNG ---
            Console.OutputEncoding = Encoding.UTF8;
            const int MaxQuantityWord = 100;
            const long MaxFileSizeMb = 51_200;

            // --- KHỞI TẠO FILE ---
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
            var wordQuantity = InputHelper.GetValidInt($"Nhập số lượng từ xuất hiện nhiều nhất (1 - {MaxQuantityWord:N0}): ", 1, MaxQuantityWord);
            Console.WriteLine($"[OK] Hệ thống sẽ xử lý {wordQuantity:N0} từ.");

            Stopwatch sw = new Stopwatch();

            // --- ĐẾM TỪ ---
            // Xử lý tuần tự
            Console.WriteLine("\n--- Đang xử lý TUẦN TỰ ---");
            var sequential = new SequentialWordsCounter();
            sw.Restart();
            sequential.Execute(File.ReadLines(filePath)); 
            var seqResults = sequential.GetTopWords(wordQuantity);
            
            sw.Stop();
            AnalyzeLog.PrintResults(seqResults, sw.ElapsedMilliseconds, sequential.GetTotalWordsCount());

            // Xử lý song song
            Console.WriteLine("\n--- Đang xử lý SONG SONG ---");
            var parallel = new ParallelWordsCounter();
            sw.Restart();
            parallel.Execute(File.ReadLines(filePath));
            var parResults = parallel.GetTopWords(wordQuantity);
            
            sw.Stop();
            AnalyzeLog.PrintResults(parResults, sw.ElapsedMilliseconds, parallel.GetTotalWordsCount());
                    }
    }
}
