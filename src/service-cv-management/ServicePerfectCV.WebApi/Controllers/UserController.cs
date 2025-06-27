using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
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
    }
}
