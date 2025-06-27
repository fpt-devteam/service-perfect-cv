using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServicePerfectCV.Application.Configurations;
using ServicePerfectCV.Application.DTOs.Authentication;
using ServicePerfectCV.Application.DTOs.Authentication.Requests;
using ServicePerfectCV.Application.DTOs.Authentication.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(AuthService authService, IOptions<GoogleSettings> googleSettings) : ControllerBase
    {
        private readonly GoogleSettings googleSettings = googleSettings.Value;
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var response = await authService.RegisterAsync(registerRequest);
            return Ok(response);
        }
        [HttpPut("activation-account/{token}")]
        public async Task<IActionResult> ActivateAccountAsync([FromRoute] string token)
        {
            Guid userId = authService.VerifyTokenAsync(token);
            return Ok(await authService.ActivateAccountAsync(userId));
        }
        [HttpPost("resend-activation-email")]
        public async Task<IActionResult> ResendActivationEmailAsync([FromBody] ResendEmailRequest resendAEmailRequest)
        {
            await authService.SendActivationEmailAsync(resendAEmailRequest.Email);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            var response = await authService.LoginAsync(loginRequest);
            return Ok(response);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshToken refreshTokenRequest)
        {
            RefreshTokenResponse response = await authService.RefreshTokenAsync(refreshTokenRequest.RefreshTokenHash);
            return Ok(response);

        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest logoutRequest)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            await authService.LogoutAsync(refreshToken: logoutRequest.RefreshToken, userId: userId);
            return NoContent();
        }

        [HttpGet("google-login-link")]
        public IActionResult LoginGoogle([FromQuery] string? returnUrl = null)
        {
            var redirectUrl = Uri.EscapeDataString(googleSettings.RedirectUri);
            var scopes = Uri.EscapeDataString(googleSettings.Scopes);
            var state = string.IsNullOrEmpty(returnUrl) ? "" : $"?returnUrl={Uri.EscapeDataString(returnUrl)}";
            return Ok($"https://accounts.google.com/o/oauth2/v2/auth?client_id={googleSettings.ClientId}&redirect_uri={redirectUrl}&response_type=code&scope={scopes}&state={state}");
        }

        [HttpPost("signin")]
        public async Task<IActionResult> ExchangeCodeAsync([FromBody] OauthExchangeCodeRequest request)
        {


            var response = await authService.ExchangeCodeAsync(request);
            return Ok(response);
        }
    }
}