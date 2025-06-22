using Microsoft.AspNetCore.Authentication.BearerToken;
using ServicePerfectCV.Application.DTOs.Authentication;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Interfaces
{
    public interface ITokenGenerator
    {
        (string AccessToken, string RefreshToken) GenerateToken(ClaimsAccessToken claimsAccessToken);
        string GenerateAccessToken(ClaimsAccessToken claimsAccessToken);
    }
}