using System.Text.RegularExpressions;

namespace ngay_5.Utilities
{
    public class WordsUtility
    {
        private static readonly Regex _logTypeRegex = new Regex(@"\b[A-Z]+\b(?=:)", RegexOptions.Compiled);

        private static readonly HashSet<string> _validLogTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "INFO", "DEBUG", "WARN", "ERROR", "SECURITY", "SYSTEM", "FATAL", "TRACE"
        };

        public static string Extract(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return string.Empty;

            MatchCollection matches = _logTypeRegex.Matches(line);

            foreach (Match match in matches)
            {
                string potentialType = match.Value;

                if (_validLogTypes.Contains(potentialType))
                {
                    return potentialType;
                }
            }

            return string.Empty;
        }
    }
}

