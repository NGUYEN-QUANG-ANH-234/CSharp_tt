using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngay_3_toi_uu.Utilities
{
    public class LoadRecords
    {
        // Async I/O - Đọc file bất đồng bộ
        public async Task<List<string>> LoadRecordsAsync(string filePath, int limit)
        {
            var records = new List<string>(limit);

            using (var reader = new StreamReader(filePath))
            {
                string line;
                int count = 0;
                while (count < limit && (line = await reader.ReadLineAsync()) != null)
                {
                    records.Add(line);
                    count++;
                }
            }
            return records;
        }

        //// Xử lý tuần tự
        //public Dictionary<string, long> ProcessSequential(List<string> records)
        //{
        //    var dict = new Dictionary<string, long>();
        //    foreach (var line in records)
        //    {
        //        var words = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
        //        foreach (var word in words)
        //        {
        //            if (dict.ContainsKey(word)) dict[word]++;
        //            else dict[word] = 1;
        //        }
        //    }
        //    return dict;
        //}

        //// Xử lý song song bằng Parallel LINQ (Thay thế Parallel.ForEach)
        //public IDictionary<string, long> ProcessPLINQ(List<string> records)
        //{
        //    return records.AsParallel()
        //        .WithDegreeOfParallelism(Environment.ProcessorCount)
        //        .SelectMany(line => line.Split(Separators, StringSplitOptions.RemoveEmptyEntries))
        //        .GroupBy(word => word)
        //        .ToDictionary(g => g.Key, g => (long)g.Count());
        //}
    }
}
