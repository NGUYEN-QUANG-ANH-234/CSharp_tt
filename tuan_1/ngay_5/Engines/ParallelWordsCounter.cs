using Microsoft.Extensions.Logging;
using ngay_5.Core;
using ngay_5.Utilities;
using Serilog;
using Serilog.Core;
using System.Collections.Concurrent;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ngay_5.Engines
{
    public class ParallelWordsCounter : WordsCounter
    {
        private readonly ConcurrentDictionary<string, long> _logTypes = new ConcurrentDictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        public override void Execute(ILogger logger, IEnumerable<string> lines)
        {
            if (lines == null)
            {
                logger.LogCritical("Danh sách dòng đầu vào (lines) tại xử lý SONG SONG bị null.");
                return;
            } 

            Parallel.ForEach<string, Dictionary<string, long>>(
                lines,
                () => new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase), // Khởi tạo từ điển cục bộ cho mỗi luồng
                (line, loopState, localDict) =>
                {
                    string logType = WordsUtility.Extract(line);
                    if (!string.IsNullOrEmpty(logType)) 
                    { 
                        if (localDict.ContainsKey(logType)) localDict[logType]++;
                        else localDict[logType] = 1;
                    }

                    return localDict;
                },
                (localDict) => // Gộp kết quả cục bộ vào kết quả tổng (Chỉ lock ở bước này)
                {
                    foreach (var kvp in localDict)
                    {
                        _logTypes.AddOrUpdate(kvp.Key, kvp.Value, (key, oldValue) => oldValue + kvp.Value);
                    }
                }
            );
        }

        public override IDictionary<string, long> GetResult() => _logTypes;
    }
}
