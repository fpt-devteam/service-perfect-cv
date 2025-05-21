using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Application.Interfaces;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"Sending email to {to} with subject: {subject}");
            Console.WriteLine($"Body: {body}");
        }
    }
}
