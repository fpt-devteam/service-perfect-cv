using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Education.Requests;
using ServicePerfectCV.Application.DTOs.Education.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cvs/{cvId}/educations")]
    public class EducationController : ControllerBase
    {
        private readonly EducationService _educationService;

        public EducationController(EducationService educationService)
        {
            _educationService = educationService;
        }

        /// <summary>
        /// Creates a new education
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="request">The education information to create</param>
        /// <returns>The newly created education information</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(EducationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateAsync(Guid cvId, [FromBody] CreateEducationRequest request)
        {
            var result = await _educationService.CreateAsync(cvId: cvId, request: request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing education
        /// </summary>
        /// <param name="educationId">The unique identifier of the education to update</param>
        /// <param name="request">The updated education information</param>
        /// <returns>The updated education information</returns>
        [HttpPut("{educationId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(EducationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid educationId, [FromBody] UpdateEducationRequest request)
        {
            var result = await _educationService.UpdateAsync(educationId: educationId, request: request);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves education information for a specific CV
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="query">Query parameters for pagination and sorting</param>
        /// <returns>The education details associated with the specified CV</returns>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<EducationResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ListAsync(Guid cvId, [FromQuery] EducationQuery query)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _educationService.GetByCVIdAsync(cvId: cvId, userId: userId, query: query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves education information by its ID
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="educationId">The unique identifier of the education</param>
        /// <returns>The education details</returns>
        [HttpGet("{educationId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(EducationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByIdAsync(Guid cvId, Guid educationId)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _educationService.GetByIdAsync(educationId: educationId, cvId: cvId, userId: userId);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an education
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="educationId">The unique identifier of the education to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{educationId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteAsync(Guid cvId, Guid educationId)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            await _educationService.DeleteAsync(educationId: educationId, cvId: cvId, userId: userId);
            return NoContent();
        }
    }
}