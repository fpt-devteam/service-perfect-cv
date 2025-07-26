using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.DTOs.Authentication;
using ServicePerfectCV.Application.DTOs.Authentication.Requests;
using ServicePerfectCV.Application.DTOs.Authentication.Responses;
using ServicePerfectCV.Application.DTOs.User.Requests;
using ServicePerfectCV.Application.DTOs.User.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class AuthService(IOptions<UrlSettings> options,
                             IOptions<JwtSettings> jwtSettings,
                             IEmailTemplateHelper helper,
                             IEmailService emailSender,
                             IPasswordHasher passwordHasher,
                             IUserRepository userRepository,
                             ITokenGenerator tokenGenerator,
                             JwtSecurityTokenHandler tokenHandler,
                             IMapper mapper,
                             IRefreshTokenService refreshTokenService,
                             IOAuthService oauthService,
                             IDeviceTokenRepository deviceTokenRepository)
    {

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            User? existingUser = await userRepository.GetByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                if (existingUser.Status == UserStatus.Inactive)
                    throw new DomainException(UserErrors.AccountNotActivated);
                throw new DomainException(UserErrors.EmailAlreadyExists);
            }
            User newUser = mapper.Map<User>(registerRequest);
            newUser.PasswordHash = passwordHasher.HashPassword(registerRequest.Password);
            newUser.AuthMethod = Domain.Enums.AuthenticationMethod.JWT;
            await userRepository.CreateAsync(newUser);
            await userRepository.SaveChangesAsync();
            //TODO: implement job queue for sending emails
            await SendActivationEmailAsync(newUser.Email);
            var response = mapper.Map<RegisterResponse>(newUser);
            return response;
        }

        public async Task<string> SendActivationEmailAsync(string email)
        {
            User user = await userRepository.GetByEmailAsync(email) ?? throw new DomainException(UserErrors.NotFound);
            if (user.Status == UserStatus.Active)
            {
                throw new DomainException(UserErrors.AccountAlreadyActivated);
            }
            var filePath = Path.Combine(AppContext.BaseDirectory, "Templates", "ActivationAccount.html");
            var token = tokenGenerator.GenerateAccessToken(new ClaimsAccessToken
            {
                UserId = user.Id.ToString(),
                Role = user.Role.ToString()
            });

            emailSender.SendEmailAsync(
                mail: user.Email,
                subject: Subjects.WelcomeToPerfectCV,
                body: await helper.RenderEmailTemplateAsync(filePath, new Dictionary<string, string>
                {
                    { "UserName", user.Email },
                    { "ActivationLink", $"{options.Value.FrontendBase}/{options.Value.ActivationPath}/{token}" }
                })
            );
            return token;
        }

        public Guid VerifyTokenAsync(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Value.Issuer,
                ValidAudience = jwtSettings.Value.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey)),
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;

                return !string.IsNullOrEmpty(userIdClaim) &&
                       !string.IsNullOrEmpty(roleClaim) &&
                       Guid.TryParse(userIdClaim, out var parsedUserId)
                    ? parsedUserId
                    : throw new DomainException(AuthErrors.InvalidActivationToken);
            }
            catch
            {
                throw new DomainException(AuthErrors.InvalidActivationToken);
            }
        }

        public async Task<bool> ActivateAccountAsync(Guid userId)
        {
            User user = await userRepository.GetByIdAsync(userId) ?? throw new DomainException(UserErrors.NotFound);
            if (user.Status == UserStatus.Active)
            {
                throw new DomainException(UserErrors.AccountAlreadyActivated);
            }
            user.Status = UserStatus.Active;
            await userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            User user = await userRepository.GetByEmailAsync(loginRequest.Email)
                ?? throw new DomainException(AuthErrors.InvalidCredential);
            if (user.Status != UserStatus.Active)
                throw new DomainException(UserErrors.AccountNotActivated);
            if (!passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash))
                throw new DomainException(AuthErrors.InvalidCredential);
            (string AccessToken, string RefreshToken) tokens = tokenGenerator.GenerateToken(new ClaimsAccessToken
            {
                UserId = user.Id.ToString(),
                Role = user.Role.ToString()
            });
            await refreshTokenService.SaveAsync(tokens.RefreshToken, user.Id.ToString());

            if (!string.IsNullOrWhiteSpace(loginRequest.DeviceToken))
            {
                var existing = await deviceTokenRepository.GetByTokenAsync(loginRequest.DeviceToken);
                if (existing == null)
                {
                    await deviceTokenRepository.CreateAsync(new DeviceToken
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Token = loginRequest.DeviceToken,
                        Platform = loginRequest.Platform ?? DevicePlatform.Web,
                        RegisteredAt = DateTime.UtcNow
                    });
                }
                else if (existing.UserId != user.Id)
                {
                    existing.UserId = user.Id;
                    existing.Platform = loginRequest.Platform ?? existing.Platform;
                    existing.RegisteredAt = DateTime.UtcNow;
                    deviceTokenRepository.Update(existing);
                }
                await deviceTokenRepository.SaveChangesAsync();
            }
            return new LoginResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
            };
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            string userId = await refreshTokenService.GetAsync(refreshToken) ?? throw new DomainException(AuthErrors.RefreshTokenInvalid);
            User user = await userRepository.GetByIdAsync(Guid.Parse(userId)) ?? throw new DomainException(UserErrors.NotFound);
            (string AccessToken, string RefreshToken) tokens = tokenGenerator.GenerateToken(new ClaimsAccessToken
            {
                UserId = userId,
                Role = user.Role.ToString()
            });
            await refreshTokenService.RevokeAsync(refreshToken);
            await refreshTokenService.SaveAsync(tokens.RefreshToken, userId);
            return new RefreshTokenResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
            };
        }

        public async Task LogoutAsync(string refreshToken, Guid userId)
        {
            string tokenUserId = await refreshTokenService.GetAsync(refreshToken) ?? throw new DomainException(UserErrors.NotFound);
            if (userId != Guid.Parse(tokenUserId))
            {
                throw new DomainException(AuthErrors.RefreshTokenInvalid);
            }
            await refreshTokenService.RevokeAsync(refreshToken);
        }

        public async Task<LoginResponse> ExchangeCodeAsync(OauthExchangeCodeRequest request)
        {
            return await oauthService.HandleOAuthAsync(request);
        }
    }
}