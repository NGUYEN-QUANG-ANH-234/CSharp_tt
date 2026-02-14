using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace ngay_5.DataAccess
{
    internal class SyncLoadRecords
    {
        public List<string> LoadRecords(string filePath, int limit) {

            var records = new List<string>();

            try
            { 
                records = File.ReadLines(filePath).Take(limit).ToList(); 
            }
            catch (FileNotFoundException ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [SyncLoadRecords] File không tìm thấy: {filePath}. Chi tiết: {ex.Message}"); Environment.Exit(1); }
            catch (UnauthorizedAccessException ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [SyncLoadRecords] Không có quyền truy cập file. Vui lòng kiểm tra quyền Admin. Detail: {ex.Message}"); Environment.Exit(1); }
            catch (IOException ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [SyncLoadRecords] File đang bị khóa hoặc lỗi phần cứng đĩa. Detail: {ex.Message}"); Environment.Exit(1); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [FATAL] [SyncLoadRecords] Lỗi hệ thống không xác định: {ex.Message}"); Environment.Exit(1); Environment.Exit(1); }

            return records;
        }

    }
}
