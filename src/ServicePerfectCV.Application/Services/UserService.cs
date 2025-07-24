using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.DTOs.User.Requests;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class UserService(IOptions<BaseUrlSettings> options,
                             IOptions<JwtSettings> jwtSettings,
                             IEmailTemplateHelper helper,
                             IEmailService emailSender,
                             IPasswordHasher passwordHasher,
                             IUserRepository userRepository,
                             ITokenGenerator tokenGenerator,
                             JwtSecurityTokenHandler tokenHandler,
                             IMapper mapper,
                             IRefreshTokenService refreshTokenService,
                             ICacheService cacheService,
                             ILogger<UserService> logger)
    {
        private const string ResetCodePrefix = "reset_code:";
        private const int ResetCodeLength = 6;
        private const int ResetCodeExpiryMinutes = 10;
        private const string ResetPasswordTemplate = "ResetPassword.html";

        public async Task<string> ForgetPasswordAsync(ForgetPasswordRequest request)
        {
            User user = await userRepository.GetByEmailAsync(request.Email) 
                ?? throw new DomainException(UserErrors.NotFound);

            string resetCode = GenerateSecureResetCode();
            logger.LogInformation($"Generated reset code for {request.Email}: {resetCode}");

            string redisKey = GetResetCodeKey(request.Email);
            await cacheService.SetAsync(redisKey, resetCode, TimeSpan.FromMinutes(ResetCodeExpiryMinutes));

            await SendResetPasswordEmailAsync(user.Email, resetCode);

            return "Reset code has been sent to your email.";
        }

        public async Task<bool> VerifyResetCodeAsync(VerifyResetCodeRequest request)
        {
            string redisKey = $"{ResetCodePrefix}{request.Email}";
            string? storedCode = await cacheService.GetAsync<string>(redisKey);

            if (string.IsNullOrEmpty(storedCode))
            {
                throw new DomainException(UserErrors.ResetCodeNotFound);
            }

            if (storedCode != request.Code)
            {
                throw new DomainException(UserErrors.InvalidResetCode);
            }

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            // Verify the reset code first
            await VerifyResetCodeAsync(new VerifyResetCodeRequest 
            { 
                Email = request.Email, 
                Code = request.Code 
            });

            User user = await userRepository.GetByEmailAsync(request.Email) 
                ?? throw new DomainException(UserErrors.NotFound);

            // Update password
            user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
            await userRepository.UpdateAsync(user);
            await userRepository.SaveChangesAsync();

            // Delete the reset code from Redis
            string redisKey = $"{ResetCodePrefix}{request.Email}";
            await cacheService.RemoveAsync(redisKey);

            return true;
        }

        private string GetResetCodeKey(string email) => $"{ResetCodePrefix}{email}";

        private async Task SendResetPasswordEmailAsync(string email, string resetCode)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", ResetPasswordTemplate);
            logger.LogInformation($"Template path: {filePath}");

            if (!File.Exists(filePath))
            {
                logger.LogError($"Template file not found at: {filePath}");
                throw new InvalidOperationException("Email template not found");
            }

            var emailBody = await helper.RenderEmailTemplateAsync(filePath, new Dictionary<string, string>
            {
                { "UserName", email },
                { "ResetCode", resetCode }
            });

            await emailSender.SendEmailAsync(
                mail: email,
                subject: "Password Reset Verification Code",
                body: emailBody
            );
        }

        private string GenerateSecureResetCode()
        {
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            int code = BitConverter.ToInt32(bytes, 0) % 900000 + 100000;
            return Math.Abs(code).ToString("D6");
        }
    }
}
