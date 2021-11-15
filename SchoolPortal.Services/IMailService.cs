using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services
{
    public interface IMailService
    {
        Task SchedulePasswordResetMail(MailObject mail);
        Task ScheduleEmailConfirmationMail(MailObject mail);
        Task<bool> SendMail(Mail mail);
        Task ProcessUnsentMails();
        Task DeleteOldMails();
    }
}
