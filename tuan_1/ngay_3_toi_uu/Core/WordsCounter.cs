using System;
using System.Collections.Generic;
using System.Linq;


namespace ngay_3_toi_uu.Core
{


    public abstract class WordsCounter
    {

        public abstract void Execute(IEnumerable<string> lines);
        public abstract IDictionary<string, long> GetResult();

        public IDictionary<string, long> GetWords(int numberOfTopWord)
        {
            // Lấy kết quả từ phương thức abstract của lớp con
            var results = GetResult();

            if (results == null || numberOfTopWord <= 0) 
            {
                if (results == null) Console.WriteLine("[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [WARN] [WordsCounter] GetTopWords: Dữ liệu nguồn rỗng.");
                else Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [WordsCounter] GetTopWords: Số lượng yêu cầu {numberOfTopWord} không hợp lệ.");
                return new Dictionary<string, long>(); 
            }

            return results
                .AsParallel()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .OrderByDescending(pair => pair.Value)
                .Take(numberOfTopWord)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public long GetTotalWordsCount()
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
