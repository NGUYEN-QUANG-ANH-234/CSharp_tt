using ngay_3_toi_uu.DataAccess;
using ngay_3_toi_uu.Engines;
using ngay_3_toi_uu.Utilities;
using ngay_3_toi_uu.Core;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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

            // --- KHỞI TẠO FILE ---
            string baseDir = AppContext.BaseDirectory;
            string filePath = Path.Combine(baseDir, "DummyLogFile.txt");

            var sizeInMb = InputHelper.GetValidLong($"Nhập dung lượng file trong khoảng 1 - {MaxFileSizeMb:N0} (MB): ", 1, MaxFileSizeMb);

            long targetSizeBytes = sizeInMb << 20;

            FileInfo file = new FileInfo(filePath);
            if (!file.Exists || file.Length < targetSizeBytes)
            {
                Console.WriteLine("[ERROR] File chưa tồn tại. Bắt đầu tạo file mới...");
                LogFileGenerator.Generate(filePath, targetSizeBytes);
            }

            Console.WriteLine($"[OK] Đã tồn tại file xấp xỉ ({sizeInMb} MB), bỏ qua bước tạo file.");

            // --- CẤU HÌNH THÔNG SỐ XỬ LÝ ---
            var lineQuantity = InputHelper.GetValidInt($"Vui lòng nhập số dòng (1 - {MaxLimit:N0}): ", 1, MaxLimit);

            var wordQuantity = InputHelper.GetValidInt($"Nhập số lượng từ xuất hiện nhiều nhất (1 - {MaxQuantityWord:N0}): ", 1, MaxQuantityWord);

            // --- LOAD FILE ---
            int recordLimit = lineQuantity;
            Stopwatch sw = Stopwatch.StartNew();

            Console.WriteLine($"--- Đang load {recordLimit:N0} records ---");

            // Sync I/O 
                        var syncProcessor = new SyncLoadRecords();
            var syncRecords = syncProcessor.LoadRecords(filePath, recordLimit);
            sw.Stop();
            Console.WriteLine($"[OK] Load Sync hoàn tất: {sw.ElapsedMilliseconds} ms\n");

            sw.Restart();

            // Async I/O 
            var asyncProcessor = new AsyncLoadRecords();
            var asyncRecords = await asyncProcessor.LoadRecords(filePath, recordLimit);
            sw.Stop();
            Console.WriteLine($"[OK] Load Async hoàn tất: {sw.ElapsedMilliseconds} ms\n");        

            sw.Restart();

            // --- ĐẾM TỪ ---
            // Xử lý tuần tự
            Console.WriteLine("\n--- Đang xử lý TUẦN TỰ ---");
            WordsCounter sequential = new SequentialWordsCounter();
            sw.Restart();
            sequential.Execute(asyncRecords);
            var seqResults = sequential.GetWords(wordQuantity);

            sw.Stop();
            LogPresenter.PrintResults(seqResults, sw.ElapsedMilliseconds, sequential.GetTotalWordsCount());

            // Xử lý song song
            Console.WriteLine("\n--- Đang xử lý SONG SONG ---");
            WordsCounter parallel = new ParallelWordsCounter();
            sw.Restart();
            parallel.Execute(asyncRecords);
            var parResults = parallel.GetWords(wordQuantity);

            sw.Stop();
            LogPresenter.PrintResults(parResults, sw.ElapsedMilliseconds, parallel.GetTotalWordsCount());
        }
    }
}