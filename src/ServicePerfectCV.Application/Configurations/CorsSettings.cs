using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Configurations
{
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; set; } = default!;
        public string[] AllowedMethods { get; set; } = default!;
        public string[] AllowedHeaders { get; set; } = default!;
        public string[] ExposedHeaders { get; set; } = default!;
        public bool AllowCredentials { get; set; }
        public int PreflightMaxAge { get; set; }
    }
}