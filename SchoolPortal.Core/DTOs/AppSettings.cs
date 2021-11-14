using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolPortal.Core.DTOs
{
    public class AppSettings
    {
        public int MaxUploadSize { get; set; }
        public int DefaultTimeZoneOffset { get; set; }
        public int ReminderServiceExecutionInterval { get; set; }
        public EmailSMTPConfig EmailSMTPConfig { get; set; }
        public int MailRetentionPeriod { get; set; }
        public int EmailVerificationTokenExpiryPeriod { get; set; }
    }
}
