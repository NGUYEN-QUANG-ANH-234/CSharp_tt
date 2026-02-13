using ngay_3_toi_uu.Core;
using ngay_3_toi_uu.Utilities;
using System;
using System.Collections.Generic;

namespace ngay_3_toi_uu.Engines
{
    public class SequentialWordsCounter : WordsCounter
    {
        private readonly Dictionary<string, long> _words = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        public override void Execute(IEnumerable<string> lines)
        {
            // Kiểm tra đầu vào (Guard Clause)
            if (lines == null) return;

            foreach (var line in lines)
            {
                string[] words = WordsUtility.Extractor(line);

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
