using ServicePerfectCV.Application.DTOs.Authentication.Requests;
using ServicePerfectCV.Application.DTOs.Authentication.Responses;
using ServicePerfectCV.Application.Interfaces;

namespace ServicePerfectCV.Infrastructure.Services.OAuth
{
    public class LinkedInOAuthService : IOAuthService
    {
        public Task<LoginResponse> HandleOAuthAsync(OauthExchangeCodeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}