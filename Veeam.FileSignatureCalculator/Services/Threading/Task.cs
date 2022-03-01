using System;
using System.Threading;

namespace Veeam.FileSignatureCalculator.Services.Threading
{
    /// <summary>
    /// Task data
    /// </summary>
    public sealed record Task(Action Action, Action InvokeBefore, Action InvokeAfter, Action<Exception> InvokeIfException, CancellationToken CancellationToken);
}
