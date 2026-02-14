using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using ngay_1_2_bo_sung.Core;
using ngay_1_2_bo_sung.Utilities;


namespace ngay_1_2_bo_sung
{
    public class ParallelWordsCounter : WordsCounter
    {
        private readonly ConcurrentDictionary<string, long> _totalWords = new ConcurrentDictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        public override void Execute(IEnumerable<string> lines)
        {
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
