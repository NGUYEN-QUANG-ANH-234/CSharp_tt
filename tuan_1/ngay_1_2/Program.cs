using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics; // Để dùng Stopwatch đo thời gian

namespace ngay_1_2
{
        internal class Program
    {

        // Ham tao log file gia lap
        static void GenerateDummyLogFile(string path, long sizeInMb) 
        {
            string[] samples =
            {
                // INFO
                "INFO: User logged in successfully",
                "INFO: User logged out",
                "INFO: New user registered",
                "INFO: Configuration loaded",
                "INFO: Scheduled task started",
                "INFO: Scheduled task completed",

                // DEBUG
                "DEBUG: Query executed successfully",
                "DEBUG: Cache hit for key user_profile",
                "DEBUG: Cache miss, loading from database",
                "DEBUG: Request payload parsed",
                "DEBUG: Response sent to client",

                // WARN
                "WARN: Memory usage is high",
                "WARN: Disk space running low",
                "WARN: API response time exceeded threshold",
                "WARN: Retry attempt for external service",
                "WARN: Deprecated API endpoint used",

                // ERROR
                "ERROR: Database connection failed",
                "ERROR: Timeout while connecting to service",
                "ERROR: Null reference exception occurred",
                "ERROR: File not found",
                "ERROR: Unauthorized access attempt",

                // SECURITY
                "SECURITY: Invalid login attempt",
                "SECURITY: Multiple failed login attempts detected",
                "SECURITY: Token expired",
                "SECURITY: Access denied for IP address",

                // SYSTEM
                "SYSTEM: Application started",
                "SYSTEM: Application stopped",
                "SYSTEM: Service restarted",
                "SYSTEM: Health check passed",
                "SYSTEM: Health check failed"
            };

            Random rand = new Random();
            long targetSize = sizeInMb * 1024 * 1024; // Khoi tao bien muc tieu tao file gia
            long currentSize = 0; // Khoi tao bien theo doi Size cua file tao

            using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                while (currentSize < targetSize)
                {
                    string line = $"{samples[rand.Next(0, samples.Length)]} at {DateTime.Now}";
                    sw.WriteLine(line);

                    // Lay size hien tai cong voi size cua dong code (theo Byte) va size cua ky tu xuong dong
                    currentSize += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;

                    if(currentSize % (100 * 1024 * 1024) < 500 )
                    Console.WriteLine($"File đã tạo được {currentSize / (1024 * 1024)} / {sizeInMb}  MB");
                };
                
            }
            Console.WriteLine($"Đã hoàn thành tạo file {currentSize} MB");
        }


        // Ham main
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Stopwatch sw = Stopwatch.StartNew();

            Console.OutputEncoding = Encoding.UTF8;

            var filePath = @"D:\hoc_lap_trinh\c_sharp_tt\tuan_1\ngay_1_2\DummyLogFile.txt";

            Console.WriteLine("Nhập dung lượng (MB) tối thiểu cho file log muốn tạo: ");
            long sizeInMb = Convert.ToInt32(Console.ReadLine());

            FileInfo file = new FileInfo(filePath);
            if (file.Exists && file.Length == (sizeInMb * 1024 * 1024))
            {
                Console.WriteLine($"Da ton tai File xap xi ({sizeInMb} MB). Bo qua buoc tao file.");
            }
            else
            {
                Console.WriteLine("File chua co hoac bi rong. Bat dau tao file moi...");
                GenerateDummyLogFile(filePath, sizeInMb);
            }

            Console.WriteLine($"Thời gian chạy cho tao file: {sw.Elapsed}");

            var wordCounts = new ConcurrentDictionary<string, long>(StringComparer.OrdinalIgnoreCase);


            // Doc file theo kieu Lazy Loading (IEnumerable)
            var lines = File.ReadLines(filePath);


            //// Bắt đầu xử lý song song
            //Parallel.ForEach(lines, line =>
            //{
            //    // Tach cau thanh cac tu don
            //    char[] separators = { ' ', ':', ',', '.', '\t' };
            //    string[] words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);


            //    foreach (var word in words)
            //    {
            //        // Cap nhat vao Dictionary (Atomic operation)
            //        wordCounts.AddOrUpdate(word, 1, (key, oldValue) => oldValue + 1);
            //    }
            //});;

            sw.Restart();
            // Tối ưu Parallel.ForEach bằng Local State
            Parallel.ForEach<string, Dictionary<string, long>>(
                lines, // Nguồn dữ liệu
                () => new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase), // Khởi tạo Local Dictionary cho mỗi luồng
                (line, loopState, localDict) => // Thực thi xử lý trên từng dòng
                {
                    char[] separators = { ' ', '.', ',', ':', ';', '\t', '(', ')', '{', '}', '-', '_', '+', '=' };
                    string[] words = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var word in words)
                    {
                        // Tối ưu: Đếm trên Dictionary riêng của luồng (không bị lock)
                        if (localDict.ContainsKey(word))
                            localDict[word]++;
                        else
                            localDict[word] = 1;
                    }
                    return localDict; // Trả về dictionary cục bộ sau khi xong một lượt
                },
                (localDict) => // Giai đoạn Gộp (Final Reduce): Chạy khi một luồng hoàn tất công việc của nó
                {
                    foreach (var kvp in localDict)
                    {
                        // Chỉ lock ở bước cuối cùng khi gộp kết quả từ luồng vào tổng thể
                        wordCounts.AddOrUpdate(kvp.Key, kvp.Value, (key, oldValue) => oldValue + kvp.Value);
                    }
                }
            );

            //LINQ theo kieu query syntax
            var topWords = (from word in wordCounts
                           orderby word.Value descending
                           select word)                       
                           .Take(10)
                           .ToList();

            //LINQ theo kieu method syntax
            //var topWords = wordCounts
            //                .OrderByDescending(kvp => kvp.Value) // Xep theo thu tu giam dan cua so lan xuat hien
            //                .Take(10)                           // Lay top 10
            //                .ToList();                          // Dua ve list

            foreach (var item in topWords)
            {
                Console.WriteLine($"Từ '{item.Key}' xuất hiện {item.Value} lần");
            }

            sw.Stop();
            Console.WriteLine($"Thời gian chạy song song: {sw.Elapsed}");
        }
    }
}
