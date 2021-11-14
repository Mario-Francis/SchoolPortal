using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NUglify;
using SchoolPortal.Core;
using SchoolPortal.Core.DTOs;
using SchoolPortal.Core.Extensions;
using SchoolPortal.Core.Models;
using SchoolPortal.Data.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SchoolPortal.Services.Implementations
{
    public class MailService:IMailService
    {
        private readonly AppSettings appSettings;
        private readonly ILogger<MailService> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IRepository<Mail> mailRepo;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IWebHostEnvironment hostEnvironment;

        public MailService(ILogger<MailService> logger,
            IOptions<AppSettings> appSettings,
            IServiceScopeFactory serviceScopeFactory,
            IRepository<Mail> mailRepo,
            IHttpContextAccessor contextAccessor,
             IWebHostEnvironment hostEnvironment)
        {   
            this.appSettings = appSettings.Value;
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
            this.mailRepo = mailRepo;
            this.contextAccessor = contextAccessor;
            this.hostEnvironment = hostEnvironment;
        }

        // schedule email confirmation mail
        public async Task ScheduleEmailConfirmationMail(MailObject mail, string username, string password, string token)
        {
            var url = contextAccessor.HttpContext.GetBaseUrl() + "Auth/VerifEmail?token=" + token;
            var message = $"Welcome onboard and we are glad to have you. You have just been profiled on the Caleb International School Portal and you are required to confirm your email address." +
                $"Kindly find your login credentials below.<br />" +
                 $"<ul><li><b>Username: </b> {username}</li><li><b>Password: </b> {password}</li></ul><br /> On login, you will be required to change your password. Please click the button below to confirm your email address.";

             var message2 = $"If you are having issues clicking the above button, copy and past the link below on your browser's address bar<br /><b>URL: </b><a href='{url}' target='_blank'>{url}</a>.<br /><br />" +
                $"The above confirmation link will expire in {appSettings.EmailVerificationTokenExpiryPeriod} days.";

            var templatePath = Path.Combine(hostEnvironment.WebRootPath, "templates", "email_verification_template.html");
            var htmlBody = await File.ReadAllTextAsync(templatePath);
            var baseUrl = contextAccessor.HttpContext.GetBaseUrl();
            htmlBody = htmlBody.Replace("{message}", message);
            htmlBody = htmlBody.Replace("{message2}", message2);
            htmlBody = htmlBody.Replace("{url}", url);

            htmlBody = htmlBody.Replace("{base_url}", baseUrl);

            var _mails = new List<Mail>();
            foreach (var m in mail.Recipients)
            {
                var _htmlBody = htmlBody;
                _htmlBody = _htmlBody.Replace("{name}", m.FirstName);
                var _mail = new Mail
                {
                    Body = _htmlBody,
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = DateTimeOffset.Now,
                    UpdatedBy = Constants.SYSTEM_NAME,
                    UpdatedDate = DateTimeOffset.Now,
                    Email = m.Email,
                    Subject = $"Welcome - Caleb School Portal Team"
                };
                _mails.Add(_mail);
            }
            await mailRepo.InsertBulk(_mails);
        }


        // send mail
        public async Task<bool> SendMail(Mail mail)
        {
            try
            {
                // create email message
                var email = new MimeMessage();
                var _from = new MailboxAddress(appSettings.EmailSMTPConfig.FromName, appSettings.EmailSMTPConfig.Username);
                // from
                email.From.Add(_from);
                // to
                email.To.Add(MailboxAddress.Parse(mail.Email));
                //  subject
                email.Subject = mail.Subject;
                // body
                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = mail.Body;
                bodyBuilder.TextBody = Uglify.HtmlToText(mail.Body).Code;

                email.Body = bodyBuilder.ToMessageBody();

                // send email
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(appSettings.EmailSMTPConfig.Host, appSettings.EmailSMTPConfig.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(appSettings.EmailSMTPConfig.Username, appSettings.EmailSMTPConfig.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                //await loggerService.LogException(ex);
                logger.LogError(ex, $"An error was encountered while sending mail to {mail.Email}");
                return false;
            }
        }

        // for mail background service
        public async Task ProcessUnsentMails()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var mailRepo = scope.ServiceProvider.GetRequiredService<IRepository<Mail>>();
                var unsentMails = mailRepo.GetWhere(m => !m.IsSent);

                await unsentMails.ParallelForEachAsync(async (mail) =>
                {
                    using (var _scope = serviceScopeFactory.CreateScope())
                    {
                        var _mailRepo = _scope.ServiceProvider.GetRequiredService<IRepository<Mail>>();
                        var _mailService = _scope.ServiceProvider.GetRequiredService<IMailService>();
                        var isSent = await _mailService.SendMail(mail);
                        if (isSent)
                        {
                            mail.IsSent = isSent;
                            mail.SentDate = DateTimeOffset.Now;
                            mail.UpdatedBy = Constants.SYSTEM_NAME;
                            mail.UpdatedDate = DateTimeOffset.Now;

                            await _mailRepo.Update(mail);
                        }
                    }
                }, Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2)));
            }
        }

        public async Task DeleteOldMails()
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var mailRepo = scope.ServiceProvider.GetRequiredService<IRepository<Mail>>();
                var mailRetentionPeriod = appSettings.MailRetentionPeriod;
                var now = DateTimeOffset.Now;
                await mailRepo.DeleteWhere(m => EF.Functions.DateDiffDay(now, m.CreatedDate) > mailRetentionPeriod);
            }
        }
 }
}
