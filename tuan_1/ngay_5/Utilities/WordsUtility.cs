using System.Text.RegularExpressions;

using Serilog;
using Microsoft.Extensions.Logging;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ngay_5.Utilities
{
    public class WordsUtility
    {
        private static readonly Regex logTypeRegex = new Regex(@"^[A-Z]+(?=:)", RegexOptions.Compiled);
        public static string[] Extract(ILogger logger, string line)
        {
            if (string.IsNullOrWhiteSpace(line)) 
            {
                logger.LogWarning("");
                return Array.Empty<string>();
            }

            var logType = logTypeRegex.Match(line);
            if (logType.Success) 
            {
                return new string[] { logType.Value };
            }
            
            return Array.Empty<string>();
                       


            //if (string.IsNullOrWhiteSpace(line)) 
            //{ 
            //    return Array.Empty<string>(); 
            //}

            //int colonIndex = line.IndexOf(':');

            //if (colonIndex != -1)
            //{
            //    string logType = line.Substring(0, colonIndex);

            //    return new string[] { logType };
            //}

            //return Array.Empty<string>();
        }
    }
}