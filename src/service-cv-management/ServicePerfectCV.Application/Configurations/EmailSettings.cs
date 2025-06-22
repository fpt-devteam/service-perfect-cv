using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Configurations
{
    public class EmailSettings
    {
        public int Port { get; init; }
        public string Password { get; init; } = default!;
        public string Host { get; init; } = default!;
        public string FromEmail { get; init; } = default!;

    }
}