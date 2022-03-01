using System;
using System.Collections.Generic;
using System.Threading;

namespace Veeam.FileSignatureCalculator.Services.Threading
{
    /// <summary>
    /// Thread pool
    /// </summary>
    public sealed class ThreadPool : IDisposable
    {
        // Queues capacity
        public int Capacity { get; }

        // Collection of worker threads
        private readonly IReadOnlyCollection<Thread> _threads;

        // Queue of actions to be processed by worker threads
        private readonly Queue<Task> _tasks;

        // Set to true when disposing queue but there are tasks to wait finish
        private bool _isDisposing;

        // Set to true when disposing queue and no more processing tasks
        private bool _isDisposed;

        private readonly object _syncObject = new();

        /// <summary>
        /// ThreadPool constructor
        /// </summary>
        /// <param name="capacity">Capacity of thread pool and processing queue</param>
        public ThreadPool(int capacity)
        {
            Capacity = capacity;
            //_cancellationTokenSource = cancellationTokenSource;

            // Create thread list with capacity
            _threads = new List<Thread>(Capacity);

            // Create tasks queue with capacity
            _tasks = new Queue<Task>(Capacity);

            var threads = new List<Thread>();

            for (var i = 0; i < capacity; i++)
            {
                var worker = new Thread(Worker);
                worker.Start();

                // Save new thread pointer
                threads.Add(worker);
            }

            _threads = threads;
        }

        /// <summary>
        /// Enqueue task to processing queue
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="invokeBefore">Action to invoke before processing task</param>
        /// <param name="invokeAfter">Action to invoke after processing task</param>
        /// <param name="invokeIfException">Action to invoke if exception occurred during processing task</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public void EnqueueTask(Action action, Action invokeBefore, Action invokeAfter, Action<Exception> invokeIfException, CancellationToken cancellationToken)
        {
            lock (_syncObject)
            {
                CheckDisposed();

                // put new task to queue
                _tasks.Enqueue(new Task(action, invokeBefore, invokeAfter, invokeIfException, cancellationToken));

                // Signal that tasks queue changed
                Monitor.PulseAll(_syncObject);
            }
        }

        /// <summary>
        /// Worker method
        /// </summary>
        private void Worker()
        {
            Task task = null;
            try
            {
                // Loop until thread pool is disposed
                while (true)
                {
                    lock (_syncObject)
                    {
                        // Wait task to process
                        while (true)
                        {
                            // Stop if thread pool disposed
                            if (_isDisposed)
                            {
                                return;
                            }

                            // Check if task available to process
                            if (_tasks.TryDequeue(out task))
                            {
                                // Signal that tasks queue changed
                                Monitor.PulseAll(_syncObject);

                                // Task to process found. Stop waiting
                                break;
                            }

                            // Wait available task
                            Monitor.Wait(_syncObject);
                        }
                    }

                    // Notify that task started to process
                    task?.InvokeBefore?.Invoke();

                    if (!task.CancellationToken.IsCancellationRequested)
                    {
                        // Process task
                        task?.Action?.Invoke();
                    }

                    //Notify that task finished to process
                    task?.InvokeAfter?.Invoke();
                }
            }
            catch (Exception ex)
            {
                task?.InvokeIfException?.Invoke(ex);

                // Wake up other threads to process cancellation
                lock (_syncObject)
                {
                    Monitor.PulseAll(_syncObject);
                }
            }
        }

        /// <summary>
        /// Throw exception if Thread pool is disposed or disposing
        /// </summary>
        private void CheckDisposed()
        {
            if (_isDisposed || _isDisposing)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Dispose thread pool
        /// </summary>
        public void Dispose()
        {
            lock (_syncObject)
            {
                if (!_isDisposed)
                {
                    // Wait for all tasks to finish processing
                    _isDisposing = true;
                    while (_tasks.Count > 0)
                    {
                        Monitor.Wait(_syncObject);
                    }

                    _isDisposed = true;

                    // Wake up all threads
                    Monitor.PulseAll(_syncObject);
                }
            }

            // Wait all threads are finished
            foreach (var worker in _threads)
            {
                worker.Join();
            }
        }
    }
}
