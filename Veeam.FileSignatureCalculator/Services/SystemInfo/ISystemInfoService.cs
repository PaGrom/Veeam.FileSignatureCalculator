namespace Veeam.FileSignatureCalculator.Services.SystemInfo
{
    /// <summary>
    /// System info service
    /// </summary>
    public interface ISystemInfoService
    {
        /// <summary>
        /// Available Memory Size in bytes
        /// </summary>
        long AvailableMemorySize { get; }

        /// <summary>
        /// Process count
        /// </summary>
        int ProcessCount { get; }
    }
}
