using System;
using System.Threading;
using Veeam.FileSignatureCalculator.Data;
using Veeam.FileSignatureCalculator.Services.FileSystem;
using Veeam.FileSignatureCalculator.Services.HashCalculator;
using Veeam.FileSignatureCalculator.Services.SystemInfo;
using Veeam.FileSignatureCalculator.Services.Threading;
using ThreadPool = Veeam.FileSignatureCalculator.Services.Threading.ThreadPool;

namespace Veeam.FileSignatureCalculator.Services.FileHashCalculator
{
    /// <summary>
    /// File's blocks hash calculator
    /// </summary>
    public sealed class FileHashCalculatorService
    {
        private readonly IHashCalculator _hashCalculator;
        private readonly ISystemInfoService _systemInfoService;

        /// <summary>
        /// FileHashCalculatorService constructor
        /// </summary>
        /// <param name="hashCalculator"><see cref="IHashCalculator"/></param>
        /// <param name="systemInfoService"><see cref="ISystemInfoService"/></param>
        public FileHashCalculatorService(IHashCalculator hashCalculator, ISystemInfoService systemInfoService)
        {
            _hashCalculator = hashCalculator;
            _systemInfoService = systemInfoService;
        }

        /// <summary>
        /// Calculate hash of file's blocks
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="blockSize">Size of block</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public void Calculate(string filePath, int blockSize, Action<BlockHash> resultCallback, CancellationToken cancellationToken)
        {
            using var threadPool = new ThreadPool(_systemInfoService.ProcessCount);
            using var fileReader = new FileReader(filePath, blockSize);
            using var enumerator = fileReader.Read().GetEnumerator();

            Func<Action> getNextFileBlockAction = () =>
            {
                while (fileReader.CanRead())
                {
                    var data = fileReader.ReadNext();
                    return () => resultCallback(CalculateHashForByteBlock(data));
                }

                return null;
            };

            var taskQueueManager = new TasksQueueManager(threadPool);
            taskQueueManager.Run(getNextFileBlockAction, cancellationToken);
        }

        /// <summary>
        /// Calculate  byte block
        /// </summary>
        /// <param name="byteBlock"><see cref="ByteBlock"/></param>
        private BlockHash CalculateHashForByteBlock(ByteBlock byteBlock)
        {
            var hash = _hashCalculator.Calculate(byteBlock.Data);
            return new BlockHash(byteBlock.BlockNumber, hash);
        }
    }
}
