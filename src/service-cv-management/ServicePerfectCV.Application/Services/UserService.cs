using Microsoft.AspNetCore.Http;
using ServicePerfectCV.Application.DTOs.User.Requests;
using ServicePerfectCV.Application.DTOs.User.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository userRepository;
        private readonly IFirebaseStorageService firebaseStorageService;
        private const string FolderStorage = "avatars";

        public UserService(IUserRepository userRepository, IFirebaseStorageService firebaseStorageService)
        {
            this.userRepository = userRepository;
            this.firebaseStorageService = firebaseStorageService;
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
    }
}
