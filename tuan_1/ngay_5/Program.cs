using ngay_5.DataAccess;
using ngay_5.Utilities;
using ngay_5.Engines;
using ngay_5.Core;

using System.Diagnostics;
using System.Text;

using Serilog;
using Microsoft.Extensions.Logging;

namespace ngay_5

{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            LoggingConfigurator.SetupSeriLog();
            using var factory = new LoggerFactory();
            var logger = factory.CreateLogger<Program>();

            try
            {
                // --- CẤU HÌNH HỆ THỐNG ---
                Console.OutputEncoding = Encoding.UTF8;
                const int MaxLimit = 2_000_000_000;
                const int MaxQuantityWord = 30;
                const long MaxFileSizeMb = 51_200;

                // --- KHỞI TẠO FILE ---
                string baseDir = AppContext.BaseDirectory;
                string filePath = Path.Combine(baseDir, "DummyLogFile.txt");

                var sizeInMb = InputHelper.GetValidLong(logger, $"Nhập dung lượng file trong khoảng 1 - {MaxFileSizeMb:N0} (MB): ", 1, MaxFileSizeMb);

                long targetSizeBytes = sizeInMb << 20;

                FileInfo file = new FileInfo(filePath);
                if (!file.Exists || file.Length < targetSizeBytes)
                {
                    logger.LogInformation("File chưa tồn tại. Bắt đầu tạo file mới...");
                    LogFileGenerator.Generate(logger, filePath, targetSizeBytes);
                }

                logger.LogInformation("Đã tồn tại file hợp lệ, xấp xỉ ({sizeMb} MB).", sizeInMb);

                // --- CẤU HÌNH THÔNG SỐ XỬ LÝ ---
                var lineQuantity = InputHelper.GetValidInt(logger, $"Vui lòng nhập số dòng (1 - {MaxLimit:N0}): ", 1, MaxLimit);

                var wordQuantity = InputHelper.GetValidInt(logger, $"Nhập số lượng loại Log cần hiển thị: ", 1, MaxQuantityWord);

                // --- LOAD FILE ---
                int recordLimit = lineQuantity;
                Stopwatch sw = Stopwatch.StartNew();

                Console.WriteLine($"--- Đang load {recordLimit:N0} records ---");

                // Sync I/O
                var syncProcessor = new SyncLoadRecords();
                var syncRecords = syncProcessor.LoadRecords(logger, filePath, recordLimit);
                sw.Stop();
                long syncIoTime = sw.ElapsedMilliseconds;
                Console.WriteLine($"Load Sync hoàn tất: {syncIoTime} ms\n");

                sw.Restart();

                // Async I/O 
                var asyncProcessor = new AsyncLoadRecords();
                var asyncRecords = await asyncProcessor.LoadRecords(logger, filePath, recordLimit);
                sw.Stop();
                long asyncIoTime = sw.ElapsedMilliseconds;
                Console.WriteLine($"Load Async hoàn tất: {asyncIoTime} ms\n");

                sw.Restart();

                // --- ĐẾM TỪ ---
                // Xử lý tuần tự
                Console.WriteLine("\n--- Đang xử lý TUẦN TỰ ---");
                WordsCounter sequential = new SequentialWordsCounter();
                sw.Restart();
                sequential.Execute(logger, asyncRecords);
                var seqResults = sequential.GetWords(logger, wordQuantity);

                long seqProcTime = sw.ElapsedMilliseconds;
                LogPresenter.PrintResults(logger, seqResults, seqProcTime, sequential.GetTotalWordsCount(logger));

                // Xử lý song song
                Console.WriteLine("\n--- Đang xử lý SONG SONG ---");
                WordsCounter parallel = new ParallelWordsCounter();
                sw.Restart();
                parallel.Execute(logger, asyncRecords);
                var parResults = parallel.GetWords(logger, wordQuantity);

                sw.Stop();
                long parProcTime = sw.ElapsedMilliseconds;
                LogPresenter.PrintResults(logger, parResults, parProcTime, parallel.GetTotalWordsCount(logger));

                // --- XUẤT BIỂU ĐỒ BÁO CÁO ---
                ChartVisualizer.Export(logger, syncIoTime, asyncIoTime, seqProcTime, parProcTime, lineQuantity);

                Console.WriteLine("\nNhấn phím bất kỳ để kết thúc...");
                Console.ReadKey();
            }
            finally
            {
                Log.CloseAndFlush(); // Quan trọng nhất để không mất log cuối
            }
        }
    }
}
