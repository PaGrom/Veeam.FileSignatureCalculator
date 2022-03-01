namespace Veeam.FileSignatureCalculator.Services.SettingsValidator
{
    /// <summary>
    /// Settings validator error messages
    /// </summary>
    public static class SettingsValidatorErrors
    {
        public const string FileNotExists = "File not exists.";

        public const string FileIsEmpty = "File is empty.";

        public const string BlockSizeTooSmall = "Block size too small.";

        public const string BlockSizeToLarge = "Block size to large. You don't have enough RAM to process file with such block size. Try to decrease value.";
    }
}
