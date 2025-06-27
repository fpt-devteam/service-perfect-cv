using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Configurations
{
    public class GoogleSettings
    {
        public string ClientId { get; init; } = default!;
        public string ClientSecret { get; init; } = default!;
        public string RedirectUri { get; init; } = default!; 
        public string Scopes { get; init; } = default!;

    }
}