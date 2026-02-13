using System;

namespace ngay_3_toi_uu.Utilities
{
    internal class WordsUtility
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
