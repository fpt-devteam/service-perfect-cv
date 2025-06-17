using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Contact.Requests;
using ServicePerfectCV.Application.Services;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/contacts")]
    public class ContactController : ControllerBase
    {
        private readonly ContactService _contactService;

        public ContactController(ContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> UpsertAsync([FromBody] UpsertContactRequest request)
        {
            var result = await _contactService.UpsertAsync(request);
            return Ok(result);
        }

        [HttpGet("cv/{cvId}")]
        public async Task<IActionResult> GetByCVIdAsync(Guid cvId)
        {
            var result = await _contactService.GetByCVIdAsync(cvId);
            return Ok(result);
        }
    }
}