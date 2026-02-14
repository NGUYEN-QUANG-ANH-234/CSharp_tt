using System;
using System.Text;

namespace ngay_5.Utilities
{
    public class WordsUtility
    {
        public static string[] Extract(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) 
            { 
                return Array.Empty<string>(); 
            }

            int colonIndex = line.IndexOf(':');

            if (colonIndex != -1)
            {
                string logType = line.Substring(0, colonIndex);

                return new string[] { logType };
            }


            return Array.Empty<string>();
        }
    }
}