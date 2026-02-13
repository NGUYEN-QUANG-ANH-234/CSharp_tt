using System;
using System.Collections.Generic;

namespace ngay_3_toi_uu.Utilities
{
    public static class AnalyzeLog
    {
        public static void PrintResults(IDictionary<string, long> data, long time, long totalWords)
        {
            if (data == null || data.Count == 0 || totalWords <= 0)
            {
                if(data == null) Console.WriteLine("[ERROR] PrintResults: Dữ liệu (Dictionary) bị null.");
                else if (totalWords <= 0) Console.WriteLine("[WARN] PrintResults: Không có từ nào được tìm thấy để tính tỷ lệ.");
                else Console.WriteLine("[INFO] PrintResults: Danh sách kết quả trống.");
                return;
            }

            foreach (var item in data)
            {
                double percentage = (double)item.Value / totalWords * 100;
                Console.WriteLine($"- {item.Key,-10}: {item.Value,10:N0} bản ghi (chiếm {percentage:N2} %)");
            }
            Console.WriteLine($"=> Tổng số từ đã đọc: {totalWords:N0}"); // Định dạng N0 để có dấu phân cách hàng nghìn
            Console.WriteLine($"=> Thời gian: {time} ms");
        }

    }
}

