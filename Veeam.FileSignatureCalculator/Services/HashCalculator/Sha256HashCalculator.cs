using System;
using System.Security.Cryptography;
using Veeam.FileSignatureCalculator.Extensions;

namespace Veeam.FileSignatureCalculator.Services.HashCalculator
{
    /// <summary>
    /// SHA256Hash calculator
    /// </summary>
    public sealed class Sha256HashCalculator : IHashCalculator, IDisposable
    {
        private readonly SHA256 _sha256Hash = SHA256.Create();

        ///<inheritdoc cref="IHashCalculator.Calculate"/>
        public string Calculate(byte[] data)
        {
            var hash = _sha256Hash.ComputeHash(data);
            var hashString = hash.ToHexString();

            return hashString;
        }

        public void Dispose()
        {
            _sha256Hash?.Dispose();
        }
    }
}
