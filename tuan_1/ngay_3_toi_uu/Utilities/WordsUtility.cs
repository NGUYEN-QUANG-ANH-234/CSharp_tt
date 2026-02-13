using System;

namespace ngay_3_toi_uu.Utilities
{
    internal class WordsUtility
    {
        private static readonly char[] _separators = { ' ', '.', ',', ':', ';', '\t', '(', ')', '{', '}', '-', '_', '+', '=' };

        public static string[] WordsExtract(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                Console.WriteLine();
                return Array.Empty<string>();
            }

            return line.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
