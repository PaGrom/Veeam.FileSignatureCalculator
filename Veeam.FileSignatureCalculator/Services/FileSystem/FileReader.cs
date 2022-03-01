using System;
using System.Collections.Generic;
using System.IO;
using Veeam.FileSignatureCalculator.Data;

namespace Veeam.FileSignatureCalculator.Services.FileSystem
{
    /// <summary>
    /// Service to read file by blocks
    /// </summary>
    public sealed class FileReader : IDisposable
    {
        private readonly FileStream _fileStream;
        private readonly int _blockSize;
        private int _currentBlockCount = 0;

        private bool _isDisposed;

        /// <summary>
        /// File reader constructor
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="blockSize">Size of block</param>
        public FileReader(string filePath, int blockSize)
        {
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _blockSize = blockSize;
        }

        /// <summary>
        /// Read file by blocks
        /// </summary>
        /// <returns>Collection of <see cref="ByteBlock"/></returns>
        public IEnumerable<ByteBlock> Read()
        {
            while (CanRead())
            {
                yield return ReadNext();
            }
        }

        /// <summary>
        /// If current position of stream not equals to eol, service can read next block
        /// </summary>
        /// <returns>Can read?</returns>
        public bool CanRead() => _fileStream.Position != _fileStream.Length;

        /// <summary>
        /// Read next byte block of file
        /// </summary>
        /// <returns><see cref="ByteBlock"/></returns>
        public ByteBlock ReadNext()
        {
            CheckDisposed();

            var bufferSize = GetNextBufferSize();

            var buffer = new byte[bufferSize];

            _fileStream.Read(buffer, 0, bufferSize);

            return new ByteBlock(_currentBlockCount++, buffer);
        }

        /// <summary>
        /// Last block's size can be different, so we have to calculate it
        /// </summary>
        /// <returns></returns>
        private int GetNextBufferSize()
        {
            var sizeToEndOfFile = _fileStream.Length - _fileStream.Position;

            return sizeToEndOfFile < _blockSize
                ? (int)sizeToEndOfFile
                : _blockSize;
        }

        /// <summary>
        /// Check if service disposed
        /// </summary>
        private void CheckDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Dispose File Reader
        /// </summary>
        public void Dispose()
        {
            _fileStream?.Dispose();
            _isDisposed = true;
        }
    }
}
