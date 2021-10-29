using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.DTOs
{
    public class AppSettings
    {
        public string UAMBaseURL { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string DefaultUAMRole { get; set; }
        public string UAMGroupId { get; set; }
        public int MaxUploadSize { get; set; }
        public int DefaultTimeZoneOffset { get; set; }
        public string MyHRAPIBaseUrl { get; set; }
        public int SyncEmployeesServiceExecutionInterval { get; set; }
        public int StatusUpdateServiceExecutionInterval { get; set; }
        public  int NotificationServiceExecutionInterval { get; set; }
        public int ReminderServiceExecutionInterval { get; set; }
        public string TokenSecret { get; set; }
        public int TokenExpiryTime { get; set; }
        public EmailSMTPConfig EmailSMTPConfig { get; set; }
        public int MailRetentionPeriod { get; set; }
        public List<int> ReminderPeriodsBefore { get; set; }
        public List<int> ReminderPeriodsAfter { get; set; }
    }
}
