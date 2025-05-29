using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Configurations
{
    public class RedisSettings
    {
        public string ConnectionString { get; init; } = default!;
        public string InstanceName { get; init; } = default!;
    }
}