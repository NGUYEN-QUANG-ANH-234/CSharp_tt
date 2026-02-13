using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

using ngay_1_2_bo_sung.Engines;
using ngay_1_2_bo_sung.Utilites;

namespace ngay_1_2_bo_sung
{
    internal class Program
    {
        static void Main(string[] args)
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

            Console.Write("Nhập số lượng Top words: ");
            if (!int.TryParse(Console.ReadLine(), out int numberOfTopWords)) return;

            Stopwatch sw = new Stopwatch();

            // --- XỬ LÝ TUẦN TỰ ---
            Console.WriteLine("\n--- Đang xử lý TUẦN TỰ ---");
            var sequential = new SequentialWordsCounter();
            sw.Restart();
            sequential.Execute(File.ReadLines(filePath)); 
            var seqResults = sequential.GetTopWords(numberOfTopWords);
            
            sw.Stop();
            PrintResults(seqResults, sw.ElapsedMilliseconds, sequential.GetTotalWordsCount());

            // --- XỬ LÝ SONG SONG (LOCAL STATE) ---
            Console.WriteLine("\n--- Đang xử lý SONG SONG ---");
            var parallel = new ParallelWordsCounter();
            sw.Restart();
            parallel.Execute(File.ReadLines(filePath));
            var parResults = parallel.GetTopWords(numberOfTopWords);
            
            sw.Stop();
            PrintResults(parResults, sw.ElapsedMilliseconds, parallel.GetTotalWordsCount());

        }

        static void PrintResults(IDictionary<string, long> data, long time, long totalWords)
        {
            foreach (var item in data) Console.WriteLine($"- {item.Key}: {item.Value}");
            Console.WriteLine($"=> Tổng số từ đã đọc: {totalWords:N0}"); // Định dạng N0 để có dấu phân cách hàng nghìn
            Console.WriteLine($"=> Thời gian: {time} ms");
        }
    }
}
