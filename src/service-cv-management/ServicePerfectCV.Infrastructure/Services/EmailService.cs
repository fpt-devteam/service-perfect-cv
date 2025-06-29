using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class EmailService(IOptions<EmailSettings> options) : IEmailService
    {
        private readonly EmailSettings _emailSettings = options.Value;


        public async Task SendEmailAsync(string email, string subject, string body)
        {
            int port = _emailSettings.Port;
            string password = _emailSettings.Password;
            string host = _emailSettings.Host;
            string fromEmail = _emailSettings.FromEmail;
            var smptClient = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);
            await smptClient.SendMailAsync(mailMessage);
        }
    }
}