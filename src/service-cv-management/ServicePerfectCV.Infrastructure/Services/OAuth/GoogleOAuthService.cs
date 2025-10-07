using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.DTOs.Authentication;
using ServicePerfectCV.Application.DTOs.Authentication.Requests;
using ServicePerfectCV.Application.DTOs.Authentication.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicePerfectCV.Infrastructure.Services.OAuth
{
    public class GoogleOAuthService(
                                    HttpClient httpClient,
                                    IOptions<GoogleSettings> googleSettings,
                                    IUserRepository userRepository,
                                    ITokenGenerator tokenGenerator,
                                    IRefreshTokenService refreshTokenService) : IOAuthService
    {
        public async Task<LoginResponse> HandleOAuthAsync(OauthExchangeCodeRequest request)
        {
            var userInfo = await GetGoogleUserInfoAsync(request.Code);

            return await GoogleLoginAsync(new GoogleLoginRequest
            {
                Email = userInfo.Email
            });
        }

        private async Task<GoogleUserInfo> GetGoogleUserInfoAsync(string authorizationCode)
        {
            var tokenRequestData = CreateGoogleTokenRequestData(authorizationCode);

            var tokenRequest = new FormUrlEncodedContent(tokenRequestData);
            var tokenResponse = await httpClient.PostAsync($"{googleSettings.Value.TokenEndpoint}", tokenRequest);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new DomainException(AuthErrors.OAuthTokenExchangeFailed);
            }

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<GoogleTokenResponse>(tokenContent);

            if (tokenData?.AccessToken == null)
            {
                throw new DomainException(AuthErrors.OAuthTokenExchangeFailed);
            }

            using var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, $"{googleSettings.Value.UserInfoEndpoint}");
            userInfoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);

            var userInfoResponse = await httpClient.SendAsync(userInfoRequest);

            if (!userInfoResponse.IsSuccessStatusCode)
            {
                throw new DomainException(AuthErrors.OAuthUserInfoFailed);
            }

            var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(userInfoContent);

            if (userInfo?.Email == null)
            {
                throw new DomainException(AuthErrors.OAuthUserInfoFailed);
            }

            return userInfo;
        }

        private Dictionary<string, string> CreateGoogleTokenRequestData(string authorizationCode)
        {
            var googleSettingsValue = googleSettings.Value ??
               throw new DomainException(AuthErrors.GoogleOAuthConfigurationMissing);
            return new Dictionary<string, string>
            {
                ["client_id"] = googleSettingsValue.ClientId,
                ["client_secret"] = googleSettingsValue.ClientSecret,
                ["code"] = authorizationCode,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = googleSettingsValue.RedirectUri
            };
        }
        private async Task<LoginResponse> GoogleLoginAsync(GoogleLoginRequest googleLoginRequest)
        {
            User? user = await userRepository.GetByEmailAsync(googleLoginRequest.Email);
            if (user != null && user.AuthMethod != Domain.Enums.AuthenticationMethod.Google)
            {
                throw new DomainException(AuthErrors.AccountExistsWithDifferentMethod);
            }
            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = googleLoginRequest.Email,
                    Status = UserStatus.Active,
                    AuthMethod = Domain.Enums.AuthenticationMethod.Google,
                    Role = UserRole.User,
                    PasswordHash = "",
                    CreatedAt = DateTime.UtcNow,
                };

                await userRepository.CreateAsync(user);
                await userRepository.SaveChangesAsync();
            }
            else if (user.Status != UserStatus.Active)
            {
                user.Status = UserStatus.Active;
                user.UpdatedAt = DateTime.UtcNow;
                await userRepository.SaveChangesAsync();
            }

            (string AccessToken, string RefreshToken) tokens = tokenGenerator.GenerateToken(new ClaimsAccessToken
            {
                UserId = user.Id.ToString(),
                Role = user.Role.ToString()
            });

            await refreshTokenService.SaveAsync(tokens.RefreshToken, user.Id.ToString());

            return new LoginResponse
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
            };
        }
    }
}