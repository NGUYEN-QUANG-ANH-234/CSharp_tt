using ngay_5.Core;
using ngay_5.Utilities;

using Serilog;
using Microsoft.Extensions.Logging;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ngay_5.Engines
{
    public class SequentialWordsCounter : WordsCounter
    {
        private readonly Dictionary<string, long> _logTypes = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        public override void Execute(ILogger logger, IEnumerable<string> lines)
        {
            if (lines == null)
            {
                logger.LogCritical("Danh sách dòng đầu vào (lines) tại xử lý TUẦN TỰ bị null.");
                return;
            }


            foreach (var line in lines)
            {
                string logType = WordsUtility.Extract(line);

                if (!string.IsNullOrEmpty(logType)) { 
                    if (_logTypes.TryGetValue(logType, out long currentCount)) 
                    {
                        _logTypes[logType] = currentCount + 1;
                    }
                    else 
                    {
                        _logTypes[logType] = 1;
                    }
                }
            }
        }

        public override IDictionary<string, long> GetResult() => _logTypes;
    }
}
