using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veeam.FileSignatureCalculator.Data;
using Veeam.FileSignatureCalculator.Services.FileHashCalculator;
using Veeam.FileSignatureCalculator.Services.HashCalculator;
using Veeam.FileSignatureCalculator.Tests.Mocks;

namespace Veeam.FileSignatureCalculator.Tests
{
    [TestClass]
    public class FileHashCalculatorTests
    {
        [TestMethod]
        [DataRow("test", 2, 2)]
        [DataRow("testtest", 2, 4)]
        [DataRow("testtest", 3, 3)]
        public void TestFileHashCalculator(string fileText, int blockSize, int expectedBlockHashCount)
        {
            var systemInfoService = new TestSystemInfoService
            {
                ProcessCount = 2,
                AvailableMemorySize = 4_000_000
            };

            var results = new ConcurrentBag<BlockHash>();

            var fileHashCalculatorService = new FileHashCalculatorService(new Sha256HashCalculator(), systemInfoService);

            var tempDir = Path.GetTempPath();
            var tempFileName = Path.GetRandomFileName();
            var tempFilePath = Path.Combine(tempDir, tempFileName);
            File.WriteAllText(tempFilePath, fileText);

            try
            {
                fileHashCalculatorService.Calculate(tempFilePath, blockSize, blockHash => results.Add(blockHash), CancellationToken.None);

                Assert.AreEqual(results.Count, expectedBlockHashCount);
            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }
    }
}
