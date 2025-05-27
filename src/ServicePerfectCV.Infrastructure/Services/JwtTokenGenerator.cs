using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.DTOs.Authentication;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Services
{
    public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
    {

        public string GenerateToken(User user)
        {
            var jwtSettings = configuration
         .GetSection("JwtSettings")
         .Get<JwtSettings>()
         ?? throw new InvalidOperationException("Missing JwtSettings");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ]),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings.ExpireMinutes.ToString())),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}