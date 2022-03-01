namespace Veeam.FileSignatureCalculator.Data
{
    /// <summary>
    /// Block of bytes
    /// </summary>
    public sealed record ByteBlock(int BlockNumber, byte[] Data);
}
