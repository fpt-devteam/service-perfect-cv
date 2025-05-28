using AutoMapper;
using ServicePerfectCV.Application.DTOs.Authentication.Requests;
using ServicePerfectCV.Application.DTOs.Authentication.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class AuthService(IPasswordHasher passwordHasher, IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
    {

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (await userRepository.GetByEmailAsync(registerRequest.Email) != null)
            {
                throw new ArgumentException("Username already exists");
            }

            User newUser = mapper.Map<User>(registerRequest);
            newUser.PasswordHash = passwordHasher.HashPassword(registerRequest.Password);

            await userRepository.CreateAsync(newUser);
            await userRepository.SaveChangesAsync();
            return new RegisterResponse
            {
                User = newUser,
                Message = "Register successfully"
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            User? user = await userRepository.GetByEmailAsync(loginRequest.Email) ?? throw new DomainException(UserErrors.NotFound); // TODO: replace with custom exception
            if (!passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash))
                throw new DomainException(UserErrors.PasswordInvalid); // TODO: replace with custom exception
            return new LoginResponse { Token = jwtTokenGenerator.GenerateToken(user) };
        }


    }
}