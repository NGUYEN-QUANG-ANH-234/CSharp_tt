using System;

namespace ngay_1_2_bo_sung.Utilities
{
    public class WordsUtility
    {
        private static readonly char[] _separators = { ' ', '.', ',', ':', ';', '\t', '(', ')', '{', '}', '-', '_', '+', '=' };

        public static string[] Extractor(string line)
        {
            // Tránh lỗi nếu dòng bị null
            if (string.IsNullOrWhiteSpace(line)) return Array.Empty<string>();
            return line.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
