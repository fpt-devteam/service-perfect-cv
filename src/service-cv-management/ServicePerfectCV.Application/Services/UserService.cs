using Microsoft.AspNetCore.Http;
using ServicePerfectCV.Application.DTOs.User.Requests;
using ServicePerfectCV.Application.DTOs.User.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace ServicePerfectCV.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository userRepository;
        private readonly IFirebaseStorageService firebaseStorageService;
        private readonly ICacheService cacheService;
        private const string FolderStorage = "avatars";

        public UserService(
            IUserRepository userRepository,
            IFirebaseStorageService firebaseStorageService,
            ICacheService cacheService
        )
        {
            this.userRepository = userRepository;
            this.firebaseStorageService = firebaseStorageService;
            this.cacheService = cacheService;
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await userRepository.GetByIdAsync(userId) ?? throw new DomainException(UserErrors.NotFound);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            userRepository.Update(user);
            await userRepository.SaveChangesAsync();
        }

        public async Task<UserResponse> GetMeAsync(Guid userId)
        {
            User user = await userRepository.GetByIdAsync(userId) ?? throw new DomainException(UserErrors.NotFound);
            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.AvatarUrl
            };
        }

        public async Task<string> UploadAvatarAsync(Guid userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new DomainException(UserErrors.NoFileUploaded);

            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new DomainException(UserErrors.NotFound);

            var url = await firebaseStorageService.UploadFileAsync(file, FolderStorage);
            user.AvatarUrl = url;
            userRepository.Update(user);
            await userRepository.SaveChangesAsync();
            return url;
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await userRepository.GetByEmailAsync(email) ?? throw new DomainException(UserErrors.NotFound);

            var code = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var cacheKey = $"reset:{code}";
            var cacheValue = user.Id.ToString();
            var expiry = TimeSpan.FromHours(1);

            await cacheService.SetAsync(cacheKey, cacheValue, expiry);

            Console.WriteLine($"[DEBUG] Password reset code for {email}: {code}");
        }

        public async Task ResetPasswordAsync(string code, string newPassword)
        {
            var cacheKey = $"reset:{code}";
            var userIdStr = await cacheService.GetAsync<string>(cacheKey);
            if (userIdStr == null)
                throw new DomainException(UserErrors.InvalidResetCode);

            if (!Guid.TryParse(userIdStr, out var userId))
                throw new DomainException(UserErrors.InvalidResetCode);

            var user = await userRepository.GetByIdAsync(userId) ?? throw new DomainException(UserErrors.NotFound);

            if (!IsPasswordStrong(newPassword))
                throw new DomainException(UserErrors.PasswordTooWeak);
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);

            userRepository.Update(user);
            await userRepository.SaveChangesAsync();

            await cacheService.RemoveAsync(cacheKey); 
        }

        private bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsLower)) return false;
            if (!password.Any(char.IsDigit)) return false;
            return true;
        }

    }
}
