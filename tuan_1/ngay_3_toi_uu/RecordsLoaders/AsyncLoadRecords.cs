using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ngay_3_toi_uu.RecordsLoaders
{
    public class AsyncLoadRecords
    {
        // Async I/O - Đọc file bất đồng bộ
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
            catch (FileNotFoundException ex) { Console.WriteLine($"[ERROR] File không tìm thấy: {filePath}. Chi tiết: {ex.Message}"); }
            catch (UnauthorizedAccessException ex) { Console.WriteLine($"[ERROR] Không có quyền truy cập file. Vui lòng kiểm tra quyền Admin. Detail: {ex.Message}"); }
            catch (IOException ex){ Console.WriteLine($"[ERROR] File đang bị khóa hoặc lỗi phần cứng đĩa. Detail: {ex.Message}"); }
            catch (Exception ex) { Console.WriteLine($"[FATAL] Lỗi hệ thống không xác định: {ex.Message}"); }

            return records;

        }
    }
}
