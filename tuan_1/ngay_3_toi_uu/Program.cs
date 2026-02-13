using ngay_3_toi_uu.RecordsLoaders;
using ngay_3_toi_uu.WordCounters;
using ngay_3_toi_uu.Utilities;

using System;
using System.Collections.Generic;
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
            Console.OutputEncoding = Encoding.UTF8;

            string baseDir = AppContext.BaseDirectory;
            string filePath = Path.Combine(baseDir, "DummyLogFile.txt");

            Console.Write("Nhập dung lượng file (MB): ");
            string input = Console.ReadLine();

            // Sử dụng TryParse để kiểm tra logic mà không ném Exception (Hiệu năng cao)
            if (!long.TryParse(input, out long sizeInMb) || sizeInMb <= 0)
            {
                // Guard Clause: Xử lý sai lệch sớm
                Console.WriteLine("Lỗi: Vui lòng nhập một số nguyên dương hợp lệ.");
                return;
            }

            // Sau khi đã an toàn, mới thực hiện dịch bit
            long targetSizeBytes = sizeInMb << 20;


            FileInfo file = new FileInfo(filePath);
            if (file.Exists && file.Length > (targetSizeBytes))
            {
                Console.WriteLine($"Da ton tai File xap xi ({sizeInMb} MB). Bo qua buoc tao file.");
            }
            else
            {
                Console.WriteLine("File chua co hoac bi rong. Bat dau tao file moi...");
                GenerateLogFile.GenerateDummyLogFile(filePath, targetSizeBytes);
            }

            Console.WriteLine("Nhập số dòng trong Records muốn xử lý: ");
            var inputLimit = Console.ReadLine();
            const int MaxLimit = 2_000_000_000;

            if (!int.TryParse(inputLimit, out int lineLimit) || lineLimit <= 0 || lineLimit > MaxLimit)
            {
                Console.WriteLine($"Lỗi: Vui lòng nhập số nguyên dương từ 1 đến {MaxLimit:N0}.");
                return;
            }

            int recordLimit = lineLimit;

            // 2. Sync I/O 
            Console.WriteLine($"--- Đang load {recordLimit:N0} records (Sync I/O) ---");
            Stopwatch sw = Stopwatch.StartNew();

            var syncProcessor = new SyncLoadRecords();
            var syncRecords = syncProcessor.LoadRecords(filePath, recordLimit);
            sw.Stop();
            Console.WriteLine($"Load Sync hoàn tất: {sw.ElapsedMilliseconds} ms\n");

            sw.Restart();

            // 3. Async I/O 
            Console.WriteLine($"--- Đang load {recordLimit:N0} records (Async I/O) ---");

            var asyncProcessor = new AsyncLoadRecords();
            var asyncRecords = await asyncProcessor.LoadRecords(filePath, recordLimit);
            sw.Stop();
            Console.WriteLine($"Load Async hoàn tất: {sw.ElapsedMilliseconds} ms\n");        

            Console.Write("Nhập số lượng Top words: ");
            if (!int.TryParse(Console.ReadLine(), out int numberOfTopWords)) return;

            sw.Stop();  

            sw.Restart();

            // 4. Benchmark Tuần tự
            Console.WriteLine("\n--- Đang xử lý TUẦN TỰ ---");
            var sequential = new SequentialWordsCounter();
            sw.Restart();
            sequential.Execute(asyncRecords);
            var seqResults = sequential.GetTopWords(numberOfTopWords);

            sw.Stop();
            AnalyzeLog.PrintResults(seqResults, sw.ElapsedMilliseconds, sequential.GetTotalWordsCount());

            // 5. Benchmark PLINQ (Song song)
            Console.WriteLine("\n--- Đang xử lý SONG SONG ---");
            var parallel = new ParallelWordsCounter();
            sw.Restart();
            parallel.Execute(asyncRecords);
            var parResults = parallel.GetTopWords(numberOfTopWords);

            sw.Stop();
            AnalyzeLog.PrintResults(parResults, sw.ElapsedMilliseconds, parallel.GetTotalWordsCount());
        }


    }
}