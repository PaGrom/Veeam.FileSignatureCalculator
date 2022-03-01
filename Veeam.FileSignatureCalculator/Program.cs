using System;
using System.Reflection;
using System.Threading;
using Veeam.FileSignatureCalculator.Services.ArgsParser;
using Veeam.FileSignatureCalculator.Services.FileHashCalculator;
using Veeam.FileSignatureCalculator.Services.HashCalculator;
using Veeam.FileSignatureCalculator.Services.SettingsValidator;
using Veeam.FileSignatureCalculator.Services.SystemInfo;

namespace Veeam.FileSignatureCalculator
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var argsParser = new ArgsParserService();
            var validator = new SettingsValidatorService(new SystemInfoService());

            if (!argsParser.TryParse(args, out var filePath, out var blockSize, out var error)
                || !validator.IsValid(filePath, blockSize, out error))
            {
                WriteErrorAndHelp(error);
                return -1;
            }

            // Create CancellationTokenSource to have availability to interrupt process
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (s, e) =>
            {
                Console.WriteLine("Canceling...");
                cts.Cancel();
                e.Cancel = true;
            };

            using var hashCalculator = new Sha256HashCalculator();
            var fileHashCalculator = new FileHashCalculatorService(hashCalculator, new SystemInfoService());
            fileHashCalculator.Calculate(filePath, blockSize, blockHash => Console.WriteLine($"Block #{blockHash.BlockNumber} Hash: {blockHash.Hash}"), cts.Token);

            if (cts.IsCancellationRequested)
            {
                return -1;
            }

            return 0;
        }

        private static void WriteErrorAndHelp(string error)
        {
            Console.WriteLine($"Error: {error}");
            Console.WriteLine();
            Console.WriteLine(HelpMessage);
        }

        private static readonly string HelpMessage = @$"{Assembly.GetEntryAssembly().GetName().Name} ({Assembly.GetEntryAssembly().GetName().Version}) by Pavel Gromov

Info:
    This utility reads input file by blocks and calculates SHA256 hash for each block

Arguments:
    -f <path> - Path to file
    -b <size> - Size of block (number of bytes)

Usage:
    {Assembly.GetEntryAssembly().GetName().Name}.exe -f C:\Temp\MyFile.txt -b 1000
";
    }
}
