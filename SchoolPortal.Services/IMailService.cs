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
        Task ScheduleEmailConfirmationMail(MailObject mail, string username, string password, string token);
        Task<bool> SendMail(Mail mail);
        Task ProcessUnsentMails();
        Task DeleteOldMails();
    }
}
