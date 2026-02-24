using Microsoft.Extensions.Logging;
using ngay_5.Engines;
using Serilog;

namespace ngay_5.test.Engines
{
    public class SequentialWordsCounterTests
    {
        // SequentialWordsCounter_Execute_Test
        // Happy Path & Edge Cases
        [Fact]
        public void Execute_LinesOfLogType_ReturnDictionaryOfLogType()
        {
            using var factory = LoggerFactory.Create(builder => builder.AddSerilog());
            var logger = factory.CreateLogger<SequentialWordsCounter>();

            // Arrange
            var counter = new SequentialWordsCounter();

            var input = new List<string>
            {
                "18:20:40 INFO: User logged in successfully",
                "INFO: User logged in successfully",
                "ABC WARN: Deprecated API endpoint used",
            };

            var expected = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
            {
                { "INFO", 2 },
                { "WARN", 1 }
            };

            // Act
            counter.Execute(logger, input);
            var result = counter.GetResult();

            // Assert
            Assert.Equal(result, expected);
        }



        // Negative Path
        [Fact]
        public void Execute_InvalidLinesOfLogType_ReturnsNullDictionary()
        {
            using var factory = LoggerFactory.Create(builder => builder.AddSerilog());
            var logger = factory.CreateLogger<SequentialWordsCounter>();

            // Arrange
            var counter = new SequentialWordsCounter();

            var input = new List<string>
            {
                "",
                "HELLO: This is not a log type",
                "12345: Just numbers",
                "info: Case sensitive check"
            };

            var expectedCount = 0;

            counter.Execute(logger, input);
            var result = counter.GetResult();

            // Assert
            Assert.Equal(result.Count, expectedCount);
        }
    }
}
