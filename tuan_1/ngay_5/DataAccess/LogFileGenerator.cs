using System;
using System.IO;
using System.Text;

namespace ngay_5.DataAccess
{
    public class LogFileGenerator
    {
        private const long _OneMbInBytes = 1024 * 1024;

        private static string[] _samples =
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

        // Ham tao log file gia lap
        public static void Generate(string filePath, long sizeInMb)
        {
            try
            {
                Random rand = new Random();
                long targetSize = sizeInMb; 
                long currentSize = 0; 


                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    while (currentSize < targetSize)
                    {
                        string line = $"{_samples[rand.Next(0, _samples.Length)]} at {DateTime.Now}";
                        sw.WriteLine(line);

                        currentSize += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;

                        if (currentSize % (100 * _OneMbInBytes) < 500)
                            Console.WriteLine($"File đã tạo được {currentSize / (_OneMbInBytes)} / {targetSize / (_OneMbInBytes)}  MB");
                    }
                    ;

                }
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [INFO] Đã hoàn thành tạo file {currentSize / (_OneMbInBytes)} MB");
            }

            catch (FileNotFoundException ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [LogFileGenerator] [Generate] File không tìm thấy: {filePath}, chi tiết: {ex.Message}"); Environment.Exit(1); }
            catch (UnauthorizedAccessException ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [LogFileGenerator] [Generate] Không có quyền truy cập file. Vui lòng kiểm tra quyền Admin, chi tiết: {ex.Message}"); Environment.Exit(1); }
            catch (IOException ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [LogFileGenerator] [Generate] File đang bị khóa hoặc lỗi phần cứng đĩa, chi tiết: {ex.Message}"); Environment.Exit(1); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [LogFileGenerator] [Generate] Lỗi hệ thống không xác định, chi tiết: {ex.Message}"); Environment.Exit(1); }

        }
    }
}
