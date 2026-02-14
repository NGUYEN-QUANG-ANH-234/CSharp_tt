using ngay_3_toi_uu.Core;
using ngay_3_toi_uu.Utilities;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ngay_3_toi_uu.Engines
{
    public class ParallelWordsCounter : WordsCounter
    {
        private readonly ConcurrentDictionary<string, long> _totalWords = new ConcurrentDictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        public override void Execute(IEnumerable<string> lines)
        {
            if (lines == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [ParallelWordsCounter] [Execute] Danh sách dòng đầu vào (lines) tại xử lý SONG SONG bị null.");
                return;
            } 


            Parallel.ForEach<string, Dictionary<string, long>>(
                lines,
                () => new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase), // Khởi tạo từ điển cục bộ cho mỗi luồng
                (line, loopState, localDict) =>
                {
                    var words = WordsUtility.Extract(line);
                    foreach (var word in words)
                    {
                        if (localDict.ContainsKey(word)) localDict[word]++;
                        else localDict[word] = 1;
                    }
                    return localDict;
                },
                (localDict) => // Gộp kết quả cục bộ vào kết quả tổng (Chỉ lock ở bước này)
                {
                    foreach (var kvp in localDict)
                    {
                        _totalWords.AddOrUpdate(kvp.Key, kvp.Value, (_, oldValue) => oldValue + kvp.Value);
                    }
                }
            );
        }

        public override IDictionary<string, long> GetResult() => _totalWords;
    }
}
