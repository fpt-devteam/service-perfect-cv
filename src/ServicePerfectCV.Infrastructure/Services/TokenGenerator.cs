using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.DTOs.Authentication;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class TokenGenerator(IOptions<JwtSettings> jwtSettings, JwtSecurityTokenHandler tokenHandler) : ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));

        public (string AccessToken, string RefreshToken) GenerateToken(ClaimsAccessToken claimsAccessToken)
        {

            return (GenerateAccessToken(claimsAccessToken), GenerateRefreshToken());
        }
        private static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator numberGenerator = RandomNumberGenerator.Create();
            numberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string GenerateAccessToken(ClaimsAccessToken claimsAccessToken)
        {
            byte[] key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, claimsAccessToken.UserId),
                    new Claim(ClaimTypes.Role, claimsAccessToken.Role)
                ]),
                Expires = DateTime.UtcNow.AddSeconds(int.Parse(_jwtSettings.Expire.ToString())),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


}