using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.PushNotification.Requests;
using ServicePerfectCV.Application.Interfaces;
using System.Net;
using System.Net.Mime;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/push-notifications")]
    public class PushNotificationController : ControllerBase
    {
        private readonly IPushNotificationService _pushNotificationService;

        public PushNotificationController(IPushNotificationService pushNotificationService)
        {
            _pushNotificationService = pushNotificationService;
        }

        /// <summary>
        /// Sends push notifications to specified device tokens
        /// </summary>
        /// <param name="request">The push notification request containing device tokens, title, and message</param>
        /// <returns>Success response if notifications were sent</returns>
        [HttpPost("send")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendAsync([FromBody] SendPushNotificationRequest request)
        {
            if (request.DeviceTokens == null || !request.DeviceTokens.Any())
            {
                return BadRequest(new { message = "At least one device token is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { message = "Title is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { message = "Message is required" });
            }

            await _pushNotificationService.SendAsync(request.DeviceTokens, request.Title, request.Message);

            return Ok(new { message = "Push notifications sent successfully" });
        }

        /// <summary>
        /// Test endpoint to verify FCM configuration
        /// </summary>
        /// <returns>Configuration status</returns>
        [HttpGet("test-config")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult TestConfig()
        {
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "firebase-service-account.json");
            var absolutePath = Path.GetFullPath(configPath);

            return Ok(new
            {
                message = "Push notification service configuration check",
                timestamp = DateTime.UtcNow,
                serviceAccountKeyExists = System.IO.File.Exists(absolutePath),
                serviceAccountKeyPath = absolutePath,
                currentDirectory = Directory.GetCurrentDirectory()
            });
        }

        /// <summary>
        /// Test endpoint to verify FCM access token generation
        /// </summary>
        /// <returns>Access token test result</returns>
        [HttpGet("test-token")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> TestToken()
        {
            try
            {
                // This will test the access token generation
                await _pushNotificationService.SendAsync(new[] { "test-token" }, "Test", "Test message");
                return Ok(new { message = "Access token generation successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Access token generation failed",
                    error = ex.Message,
                    details = ex.ToString()
                });
            }
        }
    }
}