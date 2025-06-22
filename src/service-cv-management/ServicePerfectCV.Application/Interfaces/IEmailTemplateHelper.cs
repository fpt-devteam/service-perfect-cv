using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IEmailTemplateHelper
    {
        Task<string> RenderEmailTemplateAsync(string templatePath, Dictionary<string, string> variables);
    }
}