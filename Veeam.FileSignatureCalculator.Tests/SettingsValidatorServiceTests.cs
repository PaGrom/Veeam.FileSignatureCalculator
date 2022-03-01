using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veeam.FileSignatureCalculator.Services.SettingsValidator;
using Veeam.FileSignatureCalculator.Tests.Mocks;

namespace Veeam.FileSignatureCalculator.Tests
{
    [TestClass]
    public class SettingsValidatorServiceTests
    {
        [TestMethod]
        public void TestNotExistFile()
        {
            InternalTest(16_000_000_000, 8, @"C:\Temp\test.txt", 1000, false, SettingsValidatorErrors.FileNotExists);
        }

        [TestMethod]
        public void TestEmptyFile()
        {
            InternalTestWithFile(16_000_000_000, 8, 1000, false, SettingsValidatorErrors.FileIsEmpty, true);
        }

        [TestMethod]
        public void TestBlockSizeFile()
        {
            InternalTestWithFile(16_000_000_000, 8, 1000, true, null, false);
        }

        [TestMethod]
        public void TestSmallBlockSizeFile()
        {
            InternalTestWithFile(16_000_000_000, 8, 0, false, SettingsValidatorErrors.BlockSizeTooSmall, false);
        }

        [TestMethod]
        public void TestLargeBlockSizeFile()
        {
            InternalTestWithFile(4_000_000_000, 8, 1_000_000_000, false, SettingsValidatorErrors.BlockSizeToLarge, false);
        }

        private static void InternalTestWithFile(long availableMemorySize, int processCount, int blockSize, bool expectedResult, string expectedError, bool emptyFile)
        {
            var text = emptyFile ? string.Empty : "test";

            var tempDir = Path.GetTempPath();
            var tempFileName = Path.GetRandomFileName();
            var tempFilePath = Path.Combine(tempDir, tempFileName);
            File.WriteAllText(tempFilePath, text);

            try
            {
                InternalTest(availableMemorySize, processCount, tempFilePath, blockSize, expectedResult, expectedError);
            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }

        private static void InternalTest(long availableMemorySize, int processCount, string filePath, int blockSize, bool expectedResult, string expectedError)
        {
            var systemInfoService = new TestSystemInfoService
            {
                AvailableMemorySize = availableMemorySize,
                ProcessCount = processCount
            };

            var validator = new SettingsValidatorService(systemInfoService);

            var result = validator.IsValid(filePath, blockSize, out var error);

            Assert.AreEqual(result, expectedResult);
            Assert.AreEqual(error, expectedError);
        }
    }
}
