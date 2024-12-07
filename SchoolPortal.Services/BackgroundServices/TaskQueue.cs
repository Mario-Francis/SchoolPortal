using SchoolPortal.Services.BackgroundServices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SchoolPortal.Services.BackgroundServices
{

    public class TaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue;

        public TaskQueue(int capacity)
        {
            // Capacity should be set based on the expected application load and
            // number of concurrent threads accessing the queue.            
            // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
            // which completes only when space became available. This leads to backpressure,
            // in case too many publishers/calls start accumulating.
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
        }

        public async Task QueueBackgroundWorkItemAsync(
            Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            await _queue.Writer.WriteAsync(workItem);
        }

        public IAsyncEnumerable<Func<CancellationToken, Task>> DequeueAllAsync(
            CancellationToken cancellationToken)
        {
            var workItem =  _queue.Reader.ReadAllAsync(cancellationToken);

            return workItem;
        }
    }
}
