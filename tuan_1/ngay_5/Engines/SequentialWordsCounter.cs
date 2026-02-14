using ngay_5.Core;
using ngay_5.Utilities;

using System;
using System.Collections.Generic;

namespace ngay_5.Engines
{
    public class SequentialWordsCounter : WordsCounter
    {
        private readonly Dictionary<string, long> _words = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        public override void Execute(IEnumerable<string> lines)
        {
            if (lines == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [SequentialWordsCounter] [Execute] Danh sách dòng đầu vào (lines) tại xử lý TUẦN TỰ bị null.");
                return;
            }


            foreach (var line in lines)
            {
                string[] words = WordsUtility.Extract(line);

                foreach (var word in words)
                {
                    if (_words.TryGetValue(word, out long currentCount))
                    {
                        _words[word] = currentCount + 1;
                    }
                    else
                    {
                        _words[word] = 1;
                    }
                }
            }

        }

        public override IDictionary<string, long> GetResult() => _words;
    }
}
