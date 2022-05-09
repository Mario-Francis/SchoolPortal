using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolPortal.Services.BackgroundServices
{
    public interface IBackgroundTaskQueue
    {
        Task QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem);

        //Task<Func<CancellationToken, Task<ReviewerMappingData>>> DequeueAsync(
        //    CancellationToken cancellationToken);
        IAsyncEnumerable<Func<CancellationToken, Task>> DequeueAllAsync(
           CancellationToken cancellationToken);
    }
}
