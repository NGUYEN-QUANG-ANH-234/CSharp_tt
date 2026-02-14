using System;
using System.Collections.Generic;
using System.Linq;


namespace ngay_3_toi_uu.Core
{


    public abstract class WordsCounter
    {

        public abstract void Execute(IEnumerable<string> lines);
        public abstract IDictionary<string, long> GetResult();

        public IDictionary<string, long> GetTopWords(int numberOfTopWord)
        {
            // Lấy kết quả từ phương thức abstract của lớp con
            var results = GetResult();

            if (results == null || numberOfTopWord <= 0) 
            {
                if (results == null) Console.WriteLine("[WARN] GetTopWords: Dữ liệu nguồn rỗng.");
                else Console.WriteLine($"[INFO] GetTopWords: Số lượng yêu cầu {numberOfTopWord} không hợp lệ.");
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
                Console.WriteLine("[DEBUG] GetTotalWordsCount: Dữ liệu nguồn null, không thể tính tổng.");
                return 0; 
            }

            // Cộng dồn tất cả Value trong Dictionary
            return results.Values.Sum();
        }
    }
}
