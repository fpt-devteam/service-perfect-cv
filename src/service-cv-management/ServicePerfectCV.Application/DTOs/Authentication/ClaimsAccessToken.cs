using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Authentication
{
    public class ClaimsAccessToken
    {
        public string UserId { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}