using Serilog;
using Microsoft.Extensions.Logging;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ngay_5.Utilities
{
    public static class InputHelper
    {
        public static int GetValidInt(ILogger logger, string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (!int.TryParse(input, out int result))
                {
                    logger.LogWarning("Nhập không hợp lệ số nguyên (Kiểu Int).");
                    continue;
                }

                if (result < min || result > max)
                {
                    // Sử dụng :N0 để hiển thị số như 1,000,000 thay vì 1000000
                    logger.LogWarning("Nhập số nằm ngoài khoảng {Min:N0} đến {Max:N0}.", min, max);
                    continue;
                }

                return result;
            }
        }

        public static long GetValidLong(ILogger logger, string prompt, long min, long max)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (!long.TryParse(input, out long result))
                {
                    logger.LogWarning("Nhập không hợp lệ số nguyên (Kiểu Long).");
                    continue;
                }

                if (result < min || result > max)
                {
                    logger.LogWarning("Nhập số nằm ngoài khoảng {Min:N0} đến {Max:N0}.", min, max);
                    continue;
                }

                return result;
            }
        }

        public static void GetValidTime(string prompt, string status){

        }
        
    }
}