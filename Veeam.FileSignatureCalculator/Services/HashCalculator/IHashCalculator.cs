namespace Veeam.FileSignatureCalculator.Services.HashCalculator
{
    /// <summary>
    /// Hash calculator
    /// </summary>
    public interface IHashCalculator
    {
        /// <summary>
        /// Calculate hash for data
        /// </summary>
        /// <param name="data">data</param>
        /// <returns>Hash</returns>
        string Calculate(byte[] data);
    }
}
