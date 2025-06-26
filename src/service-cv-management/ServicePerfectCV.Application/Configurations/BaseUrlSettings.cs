using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Configurations
{
    public class UrlSettings
    {
        public required string FrontendBase { get; set; }
        public required string ActivationPath { get; set; }
    }
}