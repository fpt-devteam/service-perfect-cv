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

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<UserResponse> GetMeAsync(Guid userId)
        {
            User user = await userRepository.GetByIdAsync(userId) ?? throw new DomainException(UserErrors.NotFound);
            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
