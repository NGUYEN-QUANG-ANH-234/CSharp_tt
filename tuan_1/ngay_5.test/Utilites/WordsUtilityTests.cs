using ngay_5.Engines;
using ngay_5.Utilities;
using Xunit;

namespace ngay_5.test.Utilites
{
    public class WordsUtilityTests
    {
        // WordsUtility_Extract_Test

        // Happy Path & Edge Cases
        [Theory]
        [InlineData("INFO: User logged in successfully", "INFO")]
        [InlineData("ABC WARN: Deprecated API endpoint used", "WARN")]
        [InlineData("18:20:40 INFO: User logged in successfully", "INFO")]
        public void Extract_LogLine_ReturnsExpectedLogTypes(string input, string expected)
        {
            string result = WordsUtility.Extract(input);
            Assert.Equal(expected, result);
        }

        // Negative Path
        [Theory]
        [InlineData("", "")]
        [InlineData("HELLO: This is not a log type", "")]
        [InlineData("12345: Just numbers", "")]
        [InlineData("info: Case sensitive check", "")]
        public void Extract_InvalidLogLine_ReturnsEmptyLogTypes(string input, string expected)
        {
            string result = WordsUtility.Extract(input);
            Assert.Equal(expected, result);
        }
    }
}
