using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eto.Test
{
    public static class LoremGenerator
    {
        private static readonly string[] Words = new[]
        {
            "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "sed", "do",
            "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua", "ut",
            "enim", "ad", "minim", "veniam", "quis", "nostrud", "exercitation", "ullamco", "laboris",
            "nisi", "ut", "aliquip", "ex", "ea", "commodo", "consequat", "duis", "aute", "irure", "dolor",
            "in", "reprehenderit", "in", "voluptate", "velit", "esse", "cillum", "dolore", "eu", "fugiat",
            "nulla", "pariatur", "excepteur", "sint", "occaecat", "cupidatat", "non", "proident", "sunt",
            "in", "culpa", "qui", "officia", "deserunt", "mollit", "anim", "id", "est", "laborum"
        };

        public static string Generate(int wordCount, bool randomize = true)
        {
            var random = new Random();
            var result = new List<string>();

            for (int i = 0; i < wordCount; i++)
            {
                var word = Words[randomize ? random.Next(Words.Length) : i % Words.Length];
                result.Add(word);
            }

            return string.Join(" ", result);
        }
        
        public static string GenerateLines(int lineCount, int maxWordsPerLine, bool randomize = true)
		{
			return GenerateLines(lineCount, 0, maxWordsPerLine, randomize);
		}
        public static string GenerateLines(int lineCount, int minWordsPerLine, int maxWordsPerLine, bool randomize = true)
        {
            var random = new Random();
            var result = new List<string>();

            for (int i = 0; i < lineCount; i++)
            {
                var line = Generate(random.Next(minWordsPerLine, maxWordsPerLine), randomize);
                result.Add(line);
            }

            return string.Join("\n", result);
        }
    }
}