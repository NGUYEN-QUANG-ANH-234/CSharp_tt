using System;
using System.Collections.Generic;

namespace ngay_1_2_bo_sung.Utilities
{
    public static class AnalyzeLog
    {
        public static void PrintResults(IDictionary<string, long> data, long time, long totalWords)
        {
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
