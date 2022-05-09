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
        private readonly ILogger<MailService> logger;
        private readonly IOptionsSnapshot<AppSettings> appSettingsDelegate;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IRepository<Mail> mailRepo;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IWebHostEnvironment hostEnvironment;

        public MailService(ILogger<MailService> logger,
            IOptionsSnapshot<AppSettings> appSettingsDelegate,
            IServiceScopeFactory serviceScopeFactory,
            IRepository<Mail> mailRepo,
            IHttpContextAccessor contextAccessor,
             IWebHostEnvironment hostEnvironment)
        {   
            this.logger = logger;
            this.appSettingsDelegate = appSettingsDelegate;
            this.serviceScopeFactory = serviceScopeFactory;
            this.mailRepo = mailRepo;
            this.contextAccessor = contextAccessor;
            this.hostEnvironment = hostEnvironment;
        }

        // schedule email confirmation mail
        public async Task ScheduleEmailConfirmationMail(MailObject mail)
        {
            var templatePath = Path.Combine(hostEnvironment.WebRootPath, "templates", "email_verification_template.html");
            var htmlBody = await File.ReadAllTextAsync(templatePath);
            var baseUrl = contextAccessor.HttpContext.GetBaseUrl();
            htmlBody = htmlBody.Replace("{base_url}", baseUrl);

            var _mails = new List<Mail>();
            foreach (var m in mail.Recipients)
            {
                var _htmlBody = htmlBody;
                var url = contextAccessor.HttpContext.GetBaseUrl() + "Auth/VerifyEmail?token=" + m.Token;
                var message = $"Welcome onboard and we are glad to have you. You have just been profiled on the Caleb International School Portal and you are required to confirm your email address." +
                    $"Kindly find your login credentials below.<br />" +
                     $"<ul><li><b>Username: </b> {m.Username}</li><li><b>Password: </b> {m.Password}</li></ul><br /> On login, you will be required to change your password. Please click the button below to confirm your email address.";

                var message2 = $"If you are having issues clicking the above button, copy and past the link below on your browser's address bar<br /><b>URL: </b><a style='color:#8f1b39;' href='{url}' target='_blank'>{url}</a>.<br /><br />" +
                   $"The above confirmation link will expire in {appSettingsDelegate.Value.EmailVerificationTokenExpiryPeriod} days.";

                _htmlBody = _htmlBody.Replace("{name}", m.FirstName);
                _htmlBody = _htmlBody.Replace("{message}", message);
                _htmlBody = _htmlBody.Replace("{message2}", message2);
                _htmlBody = _htmlBody.Replace("{url}", url);
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

        // schedule password reset mail
        public async Task SchedulePasswordResetMail(MailObject mail)
        {
            var templatePath = Path.Combine(hostEnvironment.WebRootPath, "templates", "mail_template.html");
            var htmlBody = await File.ReadAllTextAsync(templatePath);
            var baseUrl = contextAccessor.HttpContext.GetBaseUrl();
            htmlBody = htmlBody.Replace("{base_url}", baseUrl);

            var _mails = new List<Mail>();
            foreach (var m in mail.Recipients)
            {
                var _htmlBody = htmlBody;
                var url = contextAccessor.HttpContext.GetBaseUrl() + "Dashboard";
                var message = $"This is to notify you that your account password on the CIS Portal has been reset by your System Administrator. " +
                    $"Kindly find new password below.<br />" +
                     $"<ul><li><b>Password: </b> {m.Password}</li></ul><br /> On login, you will be required to change your password.";


                _htmlBody = _htmlBody.Replace("{name}", m.FirstName);
                _htmlBody = _htmlBody.Replace("{message}", message);
                _htmlBody = _htmlBody.Replace("{message2}", "");
                _htmlBody = _htmlBody.Replace("{action_text}", "Login to your dashboard");
                _htmlBody = _htmlBody.Replace("{action_url}", url);
                var _mail = new Mail
                {
                    Body = _htmlBody,
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = DateTimeOffset.Now,
                    UpdatedBy = Constants.SYSTEM_NAME,
                    UpdatedDate = DateTimeOffset.Now,
                    Email = m.Email,
                    Subject = $"Password Reset Alert - Caleb School Portal Team"
                };
                _mails.Add(_mail);
            }
            await mailRepo.InsertBulk(_mails);
        }

        // schedule password recovery mail
        public async Task SchedulePasswordRecoveryMail(MailObject mail)
        {
            var templatePath = Path.Combine(hostEnvironment.WebRootPath, "templates", "mail_template.html");
            var htmlBody = await File.ReadAllTextAsync(templatePath);
            var baseUrl = contextAccessor.HttpContext.GetBaseUrl();
            htmlBody = htmlBody.Replace("{base_url}", baseUrl);
            
            var _mails = new List<Mail>();
            foreach (var m in mail.Recipients)
            {
                var _htmlBody = htmlBody;
                var url = contextAccessor.HttpContext.GetBaseUrl() + "Auth/ResetPassword?token="+m.Token;
                var message = $"This is to notify you that a password recovery request was initiated on your account via your email address. " +
                    $"Click the button below to reset your password.<br />";

                var message2 = $"If you are having challenges clicking the above button, kindly copy the url below and paste on your browser's address bar.<br />" +
                    $"<b>URL: </b><a href=\"{url}\">{url}</a>";


                _htmlBody = _htmlBody.Replace("{name}", m.FirstName);
                _htmlBody = _htmlBody.Replace("{message}", message);
                _htmlBody = _htmlBody.Replace("{message2}", message2);
                _htmlBody = _htmlBody.Replace("{action_text}", "Reset Password");
                _htmlBody = _htmlBody.Replace("{action_url}", url);
                var _mail = new Mail
                {
                    Body = _htmlBody,
                    CreatedBy = Constants.SYSTEM_NAME,
                    CreatedDate = DateTimeOffset.Now,
                    UpdatedBy = Constants.SYSTEM_NAME,
                    UpdatedDate = DateTimeOffset.Now,
                    Email = m.Email,
                    Subject = $"Password Recovery Alert - Caleb School Portal Team"
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
                var _from = new MailboxAddress(appSettingsDelegate.Value.EmailSMTPConfig.FromName, appSettingsDelegate.Value.EmailSMTPConfig.Username);
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
                await smtp.ConnectAsync(appSettingsDelegate.Value.EmailSMTPConfig.Host, appSettingsDelegate.Value.EmailSMTPConfig.Port, SecureSocketOptions.Auto);
                if (appSettingsDelegate.Value.EmailSMTPConfig.AuthenticateMail)
                {
                    await smtp.AuthenticateAsync(appSettingsDelegate.Value.EmailSMTPConfig.Username, appSettingsDelegate.Value.EmailSMTPConfig.Password);
                }
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
                var mailRetentionPeriod = appSettingsDelegate.Value.MailRetentionPeriod;
                var now = DateTimeOffset.Now;
                await mailRepo.DeleteWhere(m => EF.Functions.DateDiffDay(now, m.CreatedDate) > mailRetentionPeriod);
            }
        }
 }
}
