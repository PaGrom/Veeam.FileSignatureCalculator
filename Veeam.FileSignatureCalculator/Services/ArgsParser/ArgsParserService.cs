namespace Veeam.FileSignatureCalculator.Services.ArgsParser
{
    /// <summary>
    /// Arguments parser service
    /// </summary>
    public sealed class ArgsParserService
    {
        /// <summary>
        /// Try parse arguments
        /// </summary>
        /// <param name="args">Arguments array</param>
        /// <param name="filePath">Parsed file path</param>
        /// <param name="blockSize">Parsed Block Size</param>
        /// <param name="error">Error</param>
        /// <returns>Is parsed</returns>
        public bool TryParse(string[] args, out string filePath, out int blockSize, out string error)
        {
            filePath = null;
            blockSize = 0;
            error = null;

            if (args.Length != 4)
            {
                error = ArgsParserErrors.InvalidArgumentsCount;
                return false;
            }

            for (int i = 0; i < 4; i+=2)
            {
                switch (args[i])
                {
                    case "-f":
                        filePath = args[i + 1];
                        break;
                    case "-b":
                        if (!int.TryParse(args[i + 1], out blockSize))
                        {
                            error = ArgsParserErrors.IncorrectBlockSizeParameter;
                            return false;
                        }
                        break;
                    default:
                        error = ArgsParserErrors.InvalidParameters;
                        return false;
                }
            }

            return true;
        }
    }
}
