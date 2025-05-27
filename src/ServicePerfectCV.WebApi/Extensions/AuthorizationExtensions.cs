using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ServicePerfectCV.Application.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Extensions
{
    public static class AuthorizationExtensions
    {
        public static void AddAuthorizationPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? throw new ArgumentNullException(nameof(configuration), "JwtSettings configuration section is missing or invalid.");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // Handle authentication failure
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        // Handle token validation success
                        return Task.CompletedTask;
                    }
                };

                // options.Events = new JwtBearerEvents
                //    {
                //        OnTokenValidated = async context =>
                //        {
                //            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                //            var userId = context.Principal!.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //            var jit = context.Principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                //            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jit))
                //            {
                //                context.Fail("Missing claims");
                //                return;
                //            }

                //            var lastLogout = await userRepository.GetLastLogoutAsync(Guid.Parse(userId));
                //            if (lastLogout.HasValue)
                //            {
                //                var iatClaim = context.Principal.FindFirst(JwtRegisteredClaimNames.Iat)?.Value;
                //                if (iatClaim != null && long.TryParse(iatClaim, out long iatUnix))
                //                {
                //                    var tokenIssuedAt = DateTimeOffset.FromUnixTimeSeconds(iatUnix).UtcDateTime;
                //                    if (tokenIssuedAt < lastLogout.Value)
                //                    {
                //                        context.Fail("Token has been revoked due to logout from all devices");
                //                    }
                //                }
                //            }

                //            var blacklistService = context.HttpContext.RequestServices.GetRequiredService<ITokenBlacklistedStore>();
                //            if (await blacklistService.IsBlacklistedAsync(userId, jit))
                //            {
                //                context.Fail("Token has been revoked");
                //            }
                //        }
                //    };

            });
        }
    }
}