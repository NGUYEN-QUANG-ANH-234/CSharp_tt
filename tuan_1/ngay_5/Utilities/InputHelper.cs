using System;

namespace ngay_5.Utilities
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
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [InputHelper] [GetValidInt] Nhập sai số nguyên hợp lệ.");
                    continue;
                }

                if (result < min || result > max)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [InputHelper] [GetValidInt] Nhập số nằm ngoài khoảng {min:N0} đến {max:N0}.");
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
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [InputHelper] [GetValidLong] Nhập sai số nguyên hợp lệ.");
                    continue;
                }

                if (result < min || result > max)
                {
                    // Sử dụng :N0 để hiển thị số như 1,000,000 thay vì 1000000
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [InputHelper] [GetValidLong] Nhập số nằm ngoài khoảng {min:N0} đến {max:N0}.");
                    continue;
                }

                return result;
            }
        }

        public static void GetValidTime(string prompt, string status){

        }
        
    }
}