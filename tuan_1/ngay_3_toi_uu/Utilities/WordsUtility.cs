using System;
using System.Text;

namespace ngay_3_toi_uu.Utilities
{
    public class WordsUtility
    {
        public static string[] Extract(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return Array.Empty<string>();

            StringBuilder sb = new StringBuilder();

            foreach (char c in line)
            {
                if (char.IsLetter(c))
                {
                    sb.Append(c);
                }
                else if (char.IsWhiteSpace(c))
                {
                    sb.Append(' ');
                }
            }

            return sb.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}