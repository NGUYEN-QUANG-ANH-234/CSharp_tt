using ngay_5.DataAccess;
using ngay_5.Utilities;
using ngay_5.Engines;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ngay_5

{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // --- CẤU HÌNH HỆ THỐNG ---
            Console.OutputEncoding = Encoding.UTF8;
            const int MaxLimit = 2_000_000_000;
            const int MaxQuantityWord = 30;
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

            var wordQuantity = InputHelper.GetValidInt($"Nhập số lượng loại Log cần hiển thị: ", 1, MaxQuantityWord);

            // --- LOAD FILE ---
            int recordLimit = lineQuantity;
            Stopwatch sw = Stopwatch.StartNew();

            Console.WriteLine($"--- Đang load {recordLimit:N0} records ---");

            // Sync I/O
            var syncProcessor = new SyncLoadRecords();
            var syncRecords = syncProcessor.LoadRecords(filePath, recordLimit);
            sw.Stop();
            long syncIoTime = sw.ElapsedMilliseconds; 
            Console.WriteLine($"Load Sync hoàn tất: {syncIoTime} ms\n");

            sw.Restart();

            // Async I/O 
            var asyncProcessor = new AsyncLoadRecords();
            var asyncRecords = await asyncProcessor.LoadRecords(filePath, recordLimit);
            sw.Stop();
            long asyncIoTime = sw.ElapsedMilliseconds;
            Console.WriteLine($"Load Async hoàn tất: {asyncIoTime} ms\n");

            sw.Restart();

            // --- ĐẾM TỪ ---
            // Xử lý tuần tự
            Console.WriteLine("\n--- Đang xử lý TUẦN TỰ ---");
            var sequential = new SequentialWordsCounter();
            sw.Restart();
            sequential.Execute(asyncRecords);
            var seqResults = sequential.GetWords(wordQuantity);

            long seqProcTime = sw.ElapsedMilliseconds;
            LogPresenter.PrintResults(seqResults, seqProcTime, sequential.GetTotalWordsCount());

            // Xử lý song song
            Console.WriteLine("\n--- Đang xử lý SONG SONG ---");
            var parallel = new ParallelWordsCounter();
            sw.Restart();
            parallel.Execute(asyncRecords);
            var parResults = parallel.GetWords(wordQuantity);

            sw.Stop();
            long parProcTime = sw.ElapsedMilliseconds;
            LogPresenter.PrintResults(parResults, parProcTime, parallel.GetTotalWordsCount());

            // --- XUẤT BIỂU ĐỒ BÁO CÁO ---
            ChartVisualizer.Export(syncIoTime, asyncIoTime, seqProcTime, parProcTime, lineQuantity);

            Console.WriteLine("\nNhấn phím bất kỳ để kết thúc...");
            Console.ReadKey();
        }
    }
}
