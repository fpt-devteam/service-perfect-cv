using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Authentication.Responses
{
    public class RegisterResponse
    {
        public string Mail { get; set; } = null!;

    }
}