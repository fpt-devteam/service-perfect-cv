using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Authentication.Requests
{
    public class ActivateAccountRequest
    {
        public Guid UserId { get; set; }
    }
}