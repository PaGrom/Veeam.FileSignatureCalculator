using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadPool = Veeam.FileSignatureCalculator.Services.Threading.ThreadPool;

namespace Veeam.FileSignatureCalculator.Tests
{
    [TestClass]
    public class ThreadPoolTests
    {
        [TestMethod]
        public void TestTasksProcessing()
        {
            var tasksNumber = 1000;
            var processedTaskNumber = 0;

            Action task = () => Interlocked.Increment(ref processedTaskNumber);

            var cts = new CancellationTokenSource();

            using (var threadPool = new ThreadPool(2))
            {
                for (int i = 0; i < tasksNumber; i++)
                {
                    threadPool.EnqueueTask(task, null, null, null, cts.Token);
                }
            }

            Assert.AreEqual(tasksNumber, processedTaskNumber);
        }
    }
}
