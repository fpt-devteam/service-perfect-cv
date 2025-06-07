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

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(AuthService authService, IOptions<BaseUrlSettings> option) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var response = await authService.RegisterAsync(registerRequest);
            return Ok(response);
        }
        [HttpGet("activation-account")]
        public async Task<IActionResult> ActivateAccountAsync([FromQuery] string token)
        {
            Guid userId = authService.VerifyTokenAsync(token);
            if (await authService.ActivateAccountAsync(userId)) return Redirect(option.Value.SuccessUrl);
            return Redirect(option.Value.FailUrl);
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
            await authService.LogoutAsync(logoutRequest.RefreshToken, userId);
            return NoContent();
        }
    }
}