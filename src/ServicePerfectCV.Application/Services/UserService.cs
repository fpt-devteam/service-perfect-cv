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

        public async Task<string> ForgetPasswordAsync(ForgetPasswordRequest request)
        {
            User user = await userRepository.GetByEmailAsync(request.Email) 
                ?? throw new DomainException(UserErrors.NotFound);

            // Generate 6-digit OTP
            string resetCode = GenerateResetCode();
            logger.LogInformation($"Generated reset code for {request.Email}: {resetCode}");
            
            // Save to Redis with 10-minute expiry
            string redisKey = $"{ResetCodePrefix}{request.Email}";
            await cacheService.SetAsync(redisKey, resetCode, TimeSpan.FromMinutes(ResetCodeExpiryMinutes));

            // Send email
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ResetPassword.html");
            logger.LogInformation($"Template path: {filePath}");
            
            if (!File.Exists(filePath))
            {
                logger.LogError($"Template file not found at: {filePath}");
                throw new InvalidOperationException("Email template not found");
            }

            var emailBody = await helper.RenderEmailTemplateAsync(filePath, new Dictionary<string, string>
            {
                { "UserName", user.Email },
                { "ResetCode", resetCode }
            });

            await emailSender.SendEmailAsync(
                mail: user.Email,
                subject: "Password Reset Verification Code",
                body: emailBody
            );

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

        private string GenerateResetCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
