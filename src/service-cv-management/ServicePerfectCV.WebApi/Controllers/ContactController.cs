using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Contact.Requests;
using ServicePerfectCV.Application.DTOs.Contact.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Domain.Constants;
using System;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cvs/{cvId}/contacts")]
    public class ContactController : ControllerBase
    {
        private readonly ContactService _contactService;

        public ContactController(ContactService contactService)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Creates or updates contact information
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="request">The contact information to create or update</param>
        /// <returns>The newly created or updated contact information</returns>
        /// <remarks>
        /// This endpoint handles both creation and update operations for contact information.
        /// If the contact already exists (based on CV ID), it will be updated; otherwise, a new contact will be created.
        /// </remarks>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ContactResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpsertAsync(Guid cvId, [FromBody] UpsertContactRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _contactService.UpsertAsync(cvId: cvId, userId: userId, request: request);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves contact information for a specific CV
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <returns>The contact details associated with the specified CV</returns>
        /// <remarks>
        /// This endpoint returns all contact information linked to the provided CV ID.
        /// If no contact information is found, a NotFound response is returned.
        /// Keep old value if not provided
        /// Set null to remove
        /// </remarks>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ContactResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByCVIdAsync(Guid cvId)
        {
            var result = await _contactService.GetByCVIdAsync(cvId);
            return Ok(result);
        }
    }
}