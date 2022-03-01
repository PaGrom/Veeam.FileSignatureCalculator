using System.IO;
using Veeam.FileSignatureCalculator.Services.SystemInfo;

namespace Veeam.FileSignatureCalculator.Services.SettingsValidator
{
    /// <summary>
    /// Settings validator service
    /// </summary>
    public sealed class SettingsValidatorService
    {
        private readonly ISystemInfoService _systemInfoService;

        /// <summary>
        /// SettingsValidatorService constructor
        /// </summary>
        /// <param name="systemInfoService"><see cref="ISystemInfoService"/></param>
        public SettingsValidatorService(ISystemInfoService systemInfoService)
        {
            _systemInfoService = systemInfoService;
        }

        /// <summary>
        /// Check if settings valid
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="blockSize">Block size</param>
        /// <param name="error">Error message</param>
        /// <returns>Is settings valid</returns>
        public bool IsValid(string filePath, int blockSize, out string error)
        {
            return IsFilePathValid(filePath, out error) && IsBlockSizeValid(blockSize, out error);
        }

        /// <summary>
        /// Check if file path valid
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="error">Error message</param>
        /// <returns>Is file path valid</returns>
        private static bool IsFilePathValid(string filePath, out string error)
        {
            error = null;

            if (!File.Exists(filePath))
            {
                error = SettingsValidatorErrors.FileNotExists;
                return false;
            }

            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Length == 0)
            {
                error = SettingsValidatorErrors.FileIsEmpty;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if block size valid
        /// </summary>
        /// <param name="blockSize">Block size</param>
        /// <param name="error">Error message</param>
        /// <returns>Is block size valid</returns>
        private bool IsBlockSizeValid(int blockSize, out string error)
        {
            error = null;

            if (blockSize <= 0)
            {
                error = SettingsValidatorErrors.BlockSizeTooSmall;
                return false;
            }

            // (blockSize * _systemInfoService.ProcessCount) шы approximately how much memory utility will allocate
            // It's not good if utility allocates more than half of available memory
            if ((long)blockSize * _systemInfoService.ProcessCount > _systemInfoService.AvailableMemorySize * 0.5)
            {
                error = SettingsValidatorErrors.BlockSizeToLarge;
                return false;
            }

            return true;
        }
    }
}
