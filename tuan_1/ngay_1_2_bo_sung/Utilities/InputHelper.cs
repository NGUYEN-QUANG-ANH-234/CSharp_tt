using System;

namespace ngay_1_2_bo_sung.Utilities
{
    public static class InputHelper
    {
        public static int GetValidInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int result))
                {
                    Console.WriteLine("[ERROR] Lỗi: Vui lòng nhập một số nguyên hợp lệ.");
                    continue;
                }

                if (result < min || result > max)
                {
                    Console.WriteLine($"[ERROR] Lỗi: Số phải nằm trong khoảng {min:N0} đến {max:N0}.");
                    continue;
                }

                return result;
            }
        }

        public static long GetValidLong(string prompt, long min, long max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();

                if (!long.TryParse(input, out long result))
                {
                    Console.WriteLine("[ERROR] Lỗi: Vui lòng nhập một số nguyên hợp lệ.");
                    continue;
                }

                if (result < min || result > max)
                {
                    // Sử dụng :N0 để hiển thị số như 1,000,000 thay vì 1000000
                    Console.WriteLine($"[ERROR] Lỗi: Số phải nằm trong khoảng {min:N0} đến {max:N0}.");
                    continue;
                }

                return result;
            }
        }
    }
}