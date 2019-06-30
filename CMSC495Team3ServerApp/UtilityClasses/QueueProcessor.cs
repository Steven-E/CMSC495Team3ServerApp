using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CMSC495Team3ServerApp.UtilityClasses
{
    public class QueueProcessor<T>
    {
        private readonly BlockingCollection<T> queue;

        private readonly Action<T> workAction;
        private readonly CancellationToken cancellationToken;

        public QueueProcessor(Action<T> workAction, CancellationToken cancellationToken, int capacity = 100000)
        {
            if (workAction == null) throw new ArgumentNullException(nameof(workAction), "Action must be set.");

            queue = new BlockingCollection<T>(capacity);
            this.workAction = workAction;
            this.cancellationToken = cancellationToken;
            Task.Factory.StartNew(ProcessQueue, cancellationToken, TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void ProcessQueue()
        {
            while (!cancellationToken.IsCancellationRequested)
                try
                {
                    workAction(queue.Take(cancellationToken));
                }
                catch (OperationCanceledException)
                {
                }
        }

        public void Add(T data)
        {
            try
            {
                queue.Add(data, cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}