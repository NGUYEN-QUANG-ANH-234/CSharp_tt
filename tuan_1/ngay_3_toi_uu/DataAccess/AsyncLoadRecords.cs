using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ngay_3_toi_uu.DataAccess
{
    public class AsyncLoadRecords
    {
        public async Task<List<string>> LoadRecords(string filePath, int limit)
        {
            var records = new List<string>(limit);

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    int count = 0;
                    while (count < limit && (line = await reader.ReadLineAsync()) != null)
                    {
                        records.Add(line);
                        count++;
                    }
                }
                return records;
            }
            catch (FileNotFoundException ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [AsyncLoadRecords] File không tìm thấy: {filePath}. Chi tiết: {ex.Message}"); Environment.Exit(1); }
            catch (UnauthorizedAccessException ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [AsyncLoadRecords] Không có quyền truy cập file. Vui lòng kiểm tra quyền Admin. Detail: {ex.Message}"); Environment.Exit(1); }
            catch (IOException ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [AsyncLoadRecords] File đang bị khóa hoặc lỗi phần cứng đĩa. Detail: {ex.Message}"); Environment.Exit(1); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [AsyncLoadRecords] Lỗi hệ thống không xác định: {ex.Message}"); Environment.Exit(1); Environment.Exit(1); }

            return records;

        }
    }
}
