using System;
using System.Collections.Generic;

namespace ngay_5.Utilities
{
    public static class LogPresenter
    {
        public static void PrintResults(IDictionary<string, long> data, long time, long totalWords)
        {
            if (data == null || data.Count == 0 || totalWords <= 0)
            {
                if (data == null) Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [LogAnalyzer] [PrintResults] Dữ liệu (Dictionary) bị null.");
                else if (totalWords <= 0) Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [LogAnalyzer] [PrintResults] Không có từ nào được tìm thấy để tính tỷ lệ.");
                else Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [INFO] [LogAnalyzer] [PrintResults] Danh sách kết quả trống.");
                return;
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [INFO] [LogAnalyzer] [PrintResults] Dữ liệu (Dictionary) được đọc.");
            foreach (var item in data)
            {
                double percentage = (double)item.Value / totalWords * 100;
                Console.WriteLine($"- {item.Key,-10}: {item.Value,10:N0} bản ghi (chiếm {percentage:N2} %)");
            }
            Console.WriteLine($"=> Tổng số từ đã đọc: {totalWords:N0}");
            Console.WriteLine($"=> Thời gian: {time} ms");
        }

    }
}

