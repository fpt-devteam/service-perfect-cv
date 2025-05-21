using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IEmailSender
    {
        void SendEmail(string to, string subject, string body);
    }
}
