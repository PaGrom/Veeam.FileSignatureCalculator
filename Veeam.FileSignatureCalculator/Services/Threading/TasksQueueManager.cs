using System;
using System.Threading;

namespace Veeam.FileSignatureCalculator.Services.Threading
{
    /// <summary>
    /// Tasks queue manager
    /// </summary>
    public sealed class TasksQueueManager
    {
        // Thread pool
        private readonly ThreadPool _threadPool;
        // Thread pool capacity
        private int ThreadPoolCapacity => _threadPool.Capacity;
        // Queue size
        private int _queueSize = 0;

        // Inner exception of task
        private Exception _innerException;

        private readonly object _syncObject = new();

        /// <summary>
        /// TasksQueueManager constructor
        /// </summary>
        /// <param name="threadPool"><see cref="ThreadPool"/></param>
        public TasksQueueManager(ThreadPool threadPool)
        {
            _threadPool = threadPool;
        }

        /// <summary>
        /// Enqueue tasks to thread pool
        /// </summary>
        /// <param name="getNextTaskDelegate">Delegate to get task</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        public void Run(Func<Action> getNextTaskDelegate, CancellationToken cancellationToken)
        {
            // run if not cancelled
            while (!cancellationToken.IsCancellationRequested)
            {
                lock (_syncObject)
                {
                    // double check locking
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    // wait if queue is full
                    while (_queueSize >= ThreadPoolCapacity)
                    {
                        // stop if cancel requested
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        Monitor.Wait(_syncObject);
                    }
                }

                // throw exception if exception occurred during processing one of tasks
                if (_innerException != null)
                {
                    throw new ThreadPoolProcessingTaskException(_innerException);
                }

                // get next action
                var taskAction = getNextTaskDelegate?.Invoke();
                if (taskAction == null)
                {
                    return;
                }

                // enqueue action to thread pool
                _threadPool.EnqueueTask(taskAction, TaskStartedToProcessHandle, null, TaskThrowsExceptionHandler, cancellationToken);

                // increment queue size
                Interlocked.Increment(ref _queueSize);
            }
        }

        /// <summary>
        /// Handle TaskStartedToProcess event
        /// </summary>
        private void TaskStartedToProcessHandle()
        {
            lock (_syncObject)
            {
                // decrease queue size and notify queue size changed
                Interlocked.Decrement(ref _queueSize);
                Monitor.PulseAll(_syncObject);
            }
        }

        /// <summary>
        /// Handle TaskThrowsException event
        /// </summary>
        /// <param name="exception">Inner exception</param>
        private void TaskThrowsExceptionHandler(Exception exception)
        {
            lock (_syncObject)
            {
                // save inner exception and notify threads
                _innerException ??= exception;
                Monitor.PulseAll(_syncObject);
            }
        }
    }
}
