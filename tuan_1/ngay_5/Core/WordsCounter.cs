using Serilog;
using Microsoft.Extensions.Logging;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ngay_5.Core
{
    public abstract class WordsCounter
    {

        public abstract void Execute(ILogger logger, IEnumerable<string> lines);
        public abstract IDictionary<string, long> GetResult();

        public IDictionary<string, long> GetWords(ILogger logger, int numberOfTopWord)
        {
            // Lấy kết quả từ phương thức abstract của lớp con
            var results = GetResult();

            if (results == null || numberOfTopWord <= 0)
            {
                if (results == null) logger.LogError("Dữ liệu nguồn rỗng.");
                else logger.LogError("Số lượng yêu cầu {numberOfWord} không hợp lệ.", numberOfTopWord);

                return new Dictionary<string, long>();
            }

            return results
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .OrderByDescending(pair => pair.Value)
                .Take(numberOfTopWord)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public long GetTotalWordsCount(ILogger logger)
        {
            var results = GetResult();
            if (results == null)
            {
                Console.WriteLine("[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [WordsCounter] GetTotalWordsCount: Dữ liệu nguồn null, không thể tính tổng.");
                return 0; 
            }

            // Cộng dồn tất cả Value trong Dictionary
            return results.Values.Sum();
        }
    }
}
