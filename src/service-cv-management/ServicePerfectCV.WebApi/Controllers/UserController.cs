using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.User.Requests;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly ICacheService cacheService;

        public UserController(
            UserService userService,
            IFirebaseStorageService firebaseStorageService,
            ICacheService cacheService
            )
        {
            this.userService = userService;
            _firebaseStorageService = firebaseStorageService;
            this.cacheService = cacheService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMeAsync()
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            var response = await userService.GetMeAsync(userId: userId);
            return Ok(response);
        }

            [Authorize]
            [HttpPut("avatar")]
            public async Task<IActionResult> UploadAvatarAsync([FromForm] UploadAvatarRequest file)
            {
                var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(nameIdentifier, out var userId))
                    throw new DomainException(UserErrors.NotFound);

                var url = await userService.UploadAvatarAsync(userId, file.File);
                return Ok(new { avatarUrl = url });
            }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            await userService.UpdateProfileAsync(userId, request);
            return NoContent();
        }

        [HttpPost("password-reset-request")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            await userService.RequestPasswordResetAsync(email);
            return Ok();
        }

        [HttpPost("password-reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await userService.ResetPasswordAsync(request.Code, request.NewPassword);
            return Ok();
        }
    }
}
