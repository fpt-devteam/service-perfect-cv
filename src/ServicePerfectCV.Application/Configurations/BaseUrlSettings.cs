using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Configurations
{
    public class BaseUrlSettings
    {
        public string ActivationAccountUrl { get; set; } = string.Empty;
        public string SuccessUrl { get; set; } = string.Empty;
        public string FailUrl { get; set; } = string.Empty;
        public string ActivationAccountApi { get; set; } = string.Empty;
    }
}