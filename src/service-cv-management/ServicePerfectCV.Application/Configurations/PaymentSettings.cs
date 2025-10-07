using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Configurations
{
    public class PaymentSettings
    {
        public string PAYOS_CLIENT_ID { get; init; } = default!;
        public string PAYOS_API_KEY { get; init; } = default!;
        public string PAYOS_CHECKSUM_KEY { get; init; } = default!;
    }
}