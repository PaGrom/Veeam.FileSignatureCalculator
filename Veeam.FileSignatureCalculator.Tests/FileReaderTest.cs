using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veeam.FileSignatureCalculator.Services.FileSystem;

namespace Veeam.FileSignatureCalculator.Tests
{
    [TestClass]
    public class FileReaderTest
    {
        [TestMethod]
        [DataRow("23", 1, 2)]
        [DataRow("232", 2, 2)]
        [DataRow("2323", 2, 2)]
        public void TestFileReaderBlocksCount(string fileText, int blockSize, int expectedBlocksCount)
        {
            var tempDir = Path.GetTempPath();
            var tempFileName = Path.GetRandomFileName();
            var tempFilePath = Path.Combine(tempDir, tempFileName);
            File.WriteAllText(tempFilePath, fileText);

            try
            {
                using var fileReader = new FileReader(tempFilePath, blockSize);
                var blocksCount = fileReader.Read().Count();

                Assert.AreEqual(blocksCount, expectedBlocksCount);
            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }

        [TestMethod]
        [DataRow("23", 1, 1)]
        [DataRow("232", 2, 1)]
        [DataRow("2323", 2, 2)]
        public void TestFileReaderBlocksSize(string fileText, int blockSize, int expectedLastBlockSize)
        {
            var tempDir = Path.GetTempPath();
            var tempFileName = Path.GetRandomFileName();
            var tempFilePath = Path.Combine(tempDir, tempFileName);
            File.WriteAllText(tempFilePath, fileText);

            try
            {
                using var fileReader = new FileReader(tempFilePath, blockSize);
                var lastBlockSize = fileReader.Read().Last().Data.Length;

                Assert.AreEqual(lastBlockSize, expectedLastBlockSize);
            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }
    }
}
