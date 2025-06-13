using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.User.Requests;
using ServicePerfectCV.Application.Services;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController(UserService userService) : ControllerBase
    {
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request)
        {
            var result = await userService.ForgetPasswordAsync(request);
            return Ok(result);
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequest request)
        {
            var result = await userService.VerifyResetCodeAsync(request);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await userService.ResetPasswordAsync(request);
            return Ok(result);
        }
    }
}
