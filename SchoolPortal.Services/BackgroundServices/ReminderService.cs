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
    public class ReminderService:IHostedService, IDisposable
    {
        private readonly ILogger<ReminderService> logger;
        private readonly IOptionsMonitor<AppSettings> appSettingsDelegate;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private long executionCount = 0;
        private Timer _timer;

        public ReminderService(ILogger<ReminderService> logger,
            IOptionsMonitor<AppSettings> appSettingsDelegate,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.appSettingsDelegate = appSettingsDelegate;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Reminder background service started running.");

            var interval = appSettingsDelegate.CurrentValue.ReminderServiceExecutionInterval;
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(interval));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            if (appSettingsDelegate.CurrentValue.ReminderServiceEnabled)
            {
                _ = DoWorkAsync(state);
            }
        }
        private async Task DoWorkAsync(object state)
        {
            if (executionCount > 1000000000) executionCount = 0;
            var count = Interlocked.Increment(ref executionCount);
            logger.LogInformation("Reminder background service started executing task {Count}", count);
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();
                    //await mailService.ScheduleAssessmentReminders();
                    await Task.Delay(0);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            finally
            {
                logger.LogInformation("Reminder background service completed task {Count}", count);
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Reminder background service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
