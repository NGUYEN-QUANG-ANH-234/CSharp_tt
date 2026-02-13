using System;
using System.IO;
using System.Text;

namespace ngay_1_2_bo_sung.Utilities
{
    public class GenerateLogFile
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
        public static void GenerateDummyLogFile(string filePath, long sizeInMb)
        {
            try
            {
                Random rand = new Random();
                long targetSize = sizeInMb; // Khoi tao bien muc tieu tao file gia
                long currentSize = 0; // Khoi tao bien theo doi Size cua file tao


                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    while (currentSize < targetSize)
                    {
                        string line = $"{_samples[rand.Next(0, _samples.Length)]} at {DateTime.Now}";
                        sw.WriteLine(line);

                        // Lay size hien tai cong voi size cua dong code (theo Byte) va size cua ky tu xuong dong
                        currentSize += Encoding.UTF8.GetByteCount(line) + Environment.NewLine.Length;

                        if (currentSize % (100 * _OneMbInBytes) < 500)
                            Console.WriteLine($"File đã tạo được {currentSize / (_OneMbInBytes)} / {targetSize / (_OneMbInBytes)}  MB");
                    }
                    ;

                }
                Console.WriteLine($"Đã hoàn thành tạo file {currentSize / (_OneMbInBytes)} MB");
            }

            catch (FileNotFoundException ex) { Console.WriteLine($"[ERROR] File không tìm thấy: {filePath}. Chi tiết: {ex.Message}"); }
            catch (UnauthorizedAccessException ex) { Console.WriteLine($"[ERROR] Không có quyền truy cập file. Vui lòng kiểm tra quyền Admin. Detail: {ex.Message}"); }
            catch (IOException ex) { Console.WriteLine($"[ERROR] File đang bị khóa hoặc lỗi phần cứng đĩa. Detail: {ex.Message}"); }
            catch (Exception ex) { Console.WriteLine($"[FATAL] Lỗi hệ thống không xác định: {ex.Message}"); }

        }
    }
}
