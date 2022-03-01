using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Veeam.FileSignatureCalculator.Services.Threading;
using ThreadPool = Veeam.FileSignatureCalculator.Services.Threading.ThreadPool;

namespace Veeam.FileSignatureCalculator.Tests
{
    [TestClass]
    public class TasksQueueManagerTests
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
                var enumerator = new int[tasksNumber].GetEnumerator();

                Func<Action> getNextAction = () =>
                {
                    while (enumerator.MoveNext())
                    {
                        return task;
                    }

                    return null;
                };

                var tasksQueueManager = new TasksQueueManager(threadPool);
                tasksQueueManager.Run(getNextAction, cts.Token);
            }

            Assert.AreEqual(tasksNumber, processedTaskNumber);
        }

        [TestMethod]
        public void TestTaskException()
        {
            var exceptionMessage = "test";

            var tasksNumber = 1000;
            var processedTaskNumber = 0;

            Action task = () => Interlocked.Increment(ref processedTaskNumber);

            var cts = new CancellationTokenSource();

            using (var threadPool = new ThreadPool(2))
            {
                using var enumerator = Enumerable.Range(0, 1000).GetEnumerator();

                Func<Action> getNextAction = () =>
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current == 23)
                        {
                            return () => throw new Exception(exceptionMessage);
                        }

                        return task;
                    }

                    return null;
                };

                var tasksQueueManager = new TasksQueueManager(threadPool);
                try
                {
                    tasksQueueManager.Run(getNextAction, cts.Token);
                }
                catch (ThreadPoolProcessingTaskException ex)
                {
                    Assert.AreEqual(ex.InnerException.Message, exceptionMessage);
                }
            }

            Assert.AreNotEqual(tasksNumber, processedTaskNumber);
        }

        [TestMethod]
        public void TestCancellationToken()
        {
            var tasksNumber = 1000;
            var processedTaskNumber = 0;

            Action task = () => Interlocked.Increment(ref processedTaskNumber);

            var cts = new CancellationTokenSource();

            using (var threadPool = new ThreadPool(2))
            {
                using var enumerator = Enumerable.Range(0, 1000).GetEnumerator();

                Func<Action> getNextAction = () =>
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current == 23)
                        {
                            cts.Cancel();
                        }

                        return task;
                    }

                    return null;
                };

                var tasksQueueManager = new TasksQueueManager(threadPool);
                tasksQueueManager.Run(getNextAction, cts.Token);
            }

            Assert.AreNotEqual(tasksNumber, processedTaskNumber);
        }
    }
}
