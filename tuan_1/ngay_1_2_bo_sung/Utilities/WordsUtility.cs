using System;
using System.Collections.Generic;
using System.Text;

namespace ngay_1_2_bo_sung.Utilities
{
    public class WordsUtility
    {
        public static string[] Extract(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return Array.Empty<string>();

            StringBuilder sb = new StringBuilder();

            foreach (char c in line)
            {
                // Chỉ giữ lại chữ cái và khoảng trắng để tách từ
                // Loại bỏ hoàn toàn số (0-9) và các ký tự đặc biệt : , . -
                if (char.IsLetter(c))
                {
                    sb.Append(c);
                }
                else if (char.IsWhiteSpace(c))
                {
                    sb.Append(' ');
                }
            }

            // Tách các từ dựa trên khoảng trắng đã lọc
            return sb.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}