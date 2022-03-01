using System;
using System.IO;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Veeam.FileSignatureCalculator.Services.FileHashCalculator;
using Veeam.FileSignatureCalculator.Services.HashCalculator;
using Veeam.FileSignatureCalculator.Services.SystemInfo;

namespace Veeam.FileSignatureCalculator.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<FileHashCalculatorBenchmark>();
        }
    }

    [SimpleJob(RunStrategy.ColdStart, 5, 5, 5)]
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    public class FileHashCalculatorBenchmark
    {
        [Params(500_000_000, 5_000_000_000, 50_000_000_000)]
        public long FileSize;

        [Params(100_000, 1_000_000, 10_000_000)]
        public int BlockSize;

        private string _filePath;

        [GlobalSetup]
        public void Setup()
        {
            var tempDir = Path.GetTempPath();
            var tempFileName = Path.GetRandomFileName();
            _filePath = Path.Combine(tempDir, tempFileName);

            using var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            fileStream.SetLength(FileSize);
        }

        [Benchmark]
        public void RunHashCalc()
        {
            using var hashCalculator = new Sha256HashCalculator();
            var fileHashCalculator = new FileHashCalculatorService(hashCalculator, new SystemInfoService());
            fileHashCalculator.Calculate(_filePath, BlockSize, blockHash => Console.WriteLine($"Block #{blockHash.BlockNumber} Hash: {blockHash.Hash}"), CancellationToken.None);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }
}
