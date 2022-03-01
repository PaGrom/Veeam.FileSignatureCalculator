using System;

namespace Veeam.FileSignatureCalculator.Services.Threading
{
    /// <summary>
    /// Thread pool processing task exception
    /// </summary>
    public sealed class ThreadPoolProcessingTaskException : Exception
    {
        private const string ExceptionMessage = "An error occurred while processing the task. See the inner exception for details";

        /// <summary>
        /// ThreadPoolProcessingTaskException constructor
        /// </summary>
        /// <param name="exception">Inner exception</param>
        public ThreadPoolProcessingTaskException(Exception exception) : base(ExceptionMessage, exception) {}
    }
}
