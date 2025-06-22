using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string mail, string subject, string body);
    }
}