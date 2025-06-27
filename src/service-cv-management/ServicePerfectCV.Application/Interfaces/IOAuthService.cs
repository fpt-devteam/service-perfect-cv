using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Application.DTOs.Authentication.Requests;
using ServicePerfectCV.Application.DTOs.Authentication.Responses;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface IOAuthService
    {
        Task<LoginResponse> HandleOAuthAsync(OauthExchangeCodeRequest request);
    }
}
