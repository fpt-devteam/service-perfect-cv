using ServicePerfectCV.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Helpers
{
    public class EmailTemplateHelper : IEmailTemplateHelper
    {
        public async Task<string> RenderEmailTemplateAsync(string templatePath, Dictionary<string, string> variables)
        {
            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template file not found: {templatePath}");

            string templateContent = await File.ReadAllTextAsync(templatePath);

            foreach (var (key, val) in variables)
            {
                templateContent = templateContent.Replace($"[[${{{key}}}]]", val ?? "");
            }

            return templateContent;
        }
    }
}