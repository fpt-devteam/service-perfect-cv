using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ServicePerfectCV.Application.DTOs.Authentication;
using ServicePerfectCV.Application.DTOs.Authentication.Requests;
using ServicePerfectCV.Application.DTOs.Authentication.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class AuthService(IPasswordHasher passwordHasher, IUserRepository userRepository, ITokenGenerator tokenGenerator, IMapper mapper, IRefreshTokenService refreshTokenService)
    {

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (await userRepository.GetByEmailAsync(registerRequest.Email) != null)
            {
                throw new DomainException(UserErrors.EmailAlreadyExists);
            }
            User newUser = mapper.Map<User>(registerRequest);
            newUser.PasswordHash = passwordHasher.HashPassword(registerRequest.Password);

            await userRepository.CreateAsync(newUser);
            await userRepository.SaveChangesAsync();
            return mapper.Map<RegisterResponse>(newUser);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            User? user = await userRepository.GetByEmailAsync(loginRequest.Email) ?? throw new DomainException(UserErrors.NotFound); // TODO: replace with custom exception
            if (!passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash))
                throw new DomainException(AuthErrors.PasswordInvalid);
            (string, string) tokens = tokenGenerator.GenerateToken(new ClaimsAccessToken
            {
                UserId = user.Id.ToString(),
                Role = user.Role.ToString()
            });
            await refreshTokenService.SaveAsync(tokens.Item2.ToString(), user.Id.ToString());
            return new LoginResponse
            {
                AccessToken = tokens.Item1,
                RefreshToken = tokens.Item2,
            };
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            string? userId = await refreshTokenService.GetAsync(refreshToken) ?? throw new DomainException(AuthErrors.RefreshTokenInvalid);
            User? user = await userRepository.GetByIdAsync(Guid.Parse(userId)) ?? throw new DomainException(UserErrors.NotFound);
            (string, string) tokens = tokenGenerator.GenerateToken(new ClaimsAccessToken
            {
                UserId = userId,
                Role = user.Role.ToString()
            });
            await refreshTokenService.RevokeAsync(refreshToken);
            await refreshTokenService.SaveAsync(tokens.Item2, userId);
            return new RefreshTokenResponse
            {
                AccessToken = tokens.Item1,
                RefreshToken = tokens.Item2,
            };
        }
    }
}