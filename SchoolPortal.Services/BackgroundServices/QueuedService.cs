using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchoolPortal.Core.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolPortal.Services.BackgroundServices
{
    public class QueuedService: BackgroundService
    {
        private readonly ILogger<QueuedService> _logger;

        public QueuedService(IBackgroundTaskQueue taskQueue,
            ILogger<QueuedService> logger)
        {
            TaskQueue = taskQueue;
            _logger = logger;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"Reviewer Mapping Queued Service is running.");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItems = TaskQueue.DequeueAllAsync(stoppingToken);

                try
                {
                     await workItems.ParallelForEachAsync(async (workitem) =>
                     {
                         await workitem(stoppingToken);
                     }, Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2)));
                    //await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reviewer Mapping Queued Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
