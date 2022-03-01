namespace Veeam.FileSignatureCalculator.Data
{
    /// <summary>
    /// Block hash info
    /// </summary>
    public sealed record BlockHash(int BlockNumber, string Hash);
}
