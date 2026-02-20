using Serilog;
using Microsoft.Extensions.Logging;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ngay_5.Utilities
{
    public static class LogPresenter
    {
        public static void PrintResults(ILogger logger, IDictionary<string, long> data, long time, long totalWords)
        {
            if (data == null || data.Count == 0 || totalWords <= 0)
            {
                if (data == null)
                {
                    logger.LogError("Dữ liệu(Dictionary) bị null.");
                    throw new InvalidOperationException("File không có nội dung.");
                }
                else if (totalWords <= 0)
                {
                    logger.LogError(" Không có từ nào được tìm thấy để tính tỷ lệ.");
                    throw new InvalidOperationException("Không tìm thấy từ để tính tỉ lệ");
                }
                else
                {
                    logger.LogError("Danh sách kết quả trống.");
                    throw new InvalidOperationException("Danh sách kết quả rỗng");
                }
            }

            logger.LogInformation("Dữ liệu (Dictionary) được đọc.");
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

