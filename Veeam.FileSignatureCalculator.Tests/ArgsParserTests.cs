using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veeam.FileSignatureCalculator.Services.ArgsParser;

namespace Veeam.FileSignatureCalculator.Tests
{
    [TestClass]
    public class ArgsParserServiceTests
    {
        [TestMethod]
        [DataRow("23", false, null, 0, ArgsParserErrors.InvalidArgumentsCount)]
        [DataRow("-a aaa -b bbb", false, null, 0, ArgsParserErrors.InvalidParameters)]
        [DataRow(@"-f C:\Temp\test.txt -b aa", false, @"C:\Temp\test.txt", 0, ArgsParserErrors.IncorrectBlockSizeParameter)]
        [DataRow(@"-f C:\Temp\test.txt -b 1000", true, @"C:\Temp\test.txt", 1000, null)]
        [DataRow(@"-b 1000 -f C:\Temp\test.txt", true, @"C:\Temp\test.txt", 1000, null)]
        public void TestParser(string args, bool expectedResult, string expectedFilePath, int expectedBlockSize, string expectedError)
        {
            var argsParser = new ArgsParserService();

            var argsArray = args.Split(" ");

            var result = argsParser.TryParse(argsArray, out var filePath, out var blockSize, out var error);

            Assert.AreEqual(result, expectedResult);
            Assert.AreEqual(filePath, expectedFilePath);
            Assert.AreEqual(blockSize, expectedBlockSize);
            Assert.AreEqual(error, expectedError);
        }
    }
}
