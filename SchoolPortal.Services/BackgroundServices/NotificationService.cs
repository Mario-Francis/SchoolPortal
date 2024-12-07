using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolPortal.Core.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolPortal.Services.BackgroundServices
{
    public class NotificationService:IHostedService, IDisposable
    {
        private readonly ILogger<NotificationService> logger;
        private readonly IOptionsMonitor<AppSettings> appSettingsDelegate;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private long executionCount = 0;
        private Timer _timer;

        public NotificationService(ILogger<NotificationService> logger, 
            IOptionsMonitor<AppSettings> appSettingsDelegate, 
            IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.appSettingsDelegate = appSettingsDelegate;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Notification background service started running.");

            var interval = appSettingsDelegate.CurrentValue.NotificationServiceExecutionInterval;
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(interval));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            if (appSettingsDelegate.CurrentValue.NotificationServiceEnabled)
            {
                _ = DoWorkAsync(state);
            }
        }
        private async Task DoWorkAsync(object state)
        {
            if (executionCount > 1000000000) executionCount = 0;
            var count = Interlocked.Increment(ref executionCount);
            logger.LogInformation("Notification background service started executing task {Count}", count);
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();
                    await mailService.ProcessUnsentMails();
                    await mailService.DeleteOldMails();
                }
            }
            catch (Exception ex)
            {

                logger.LogError(ex, ex.Message);
            }
            finally
            {
                logger.LogInformation("Notification background service completed task {Count}", count);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Notification background service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
