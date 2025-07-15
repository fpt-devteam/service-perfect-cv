using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Experience.Requests;
using ServicePerfectCV.Application.DTOs.Experience.Responses;
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
    [Route("api/cvs/{cvId}/experiences")]
    public class ExperienceController : ControllerBase
    {
        private readonly ExperienceService _experienceService;

        public ExperienceController(ExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        /// <summary>
        /// Creates a new experience
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="request">The experience information to create</param>
        /// <returns>The newly created experience information</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ExperienceResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateAsync(Guid cvId, [FromBody] CreateExperienceRequest request)
        {
            var result = await _experienceService.CreateAsync(cvId: cvId, request: request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing experience
        /// </summary>
        /// <param name="experienceId">The unique identifier of the experience to update</param>
        /// <param name="request">The updated experience information</param>
        /// <returns>The updated experience information</returns>
        [HttpPut("{experienceId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ExperienceResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid experienceId, [FromBody] UpdateExperienceRequest request)
        {
            var result = await _experienceService.UpdateAsync(experienceId: experienceId, request: request);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves experience information for a specific CV
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <returns>The experience details associated with the specified CV</returns>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<ExperienceResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ListAsync(Guid cvId)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _experienceService.GetByCVIdAsync(cvId: cvId, userId: userId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves experience information by its ID
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="experienceId">The unique identifier of the experience</param>
        /// <returns>The experience details</returns>
        [HttpGet("{experienceId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ExperienceResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByIdAsync(Guid cvId, Guid experienceId)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _experienceService.GetByIdAsync(experienceId: experienceId, cvId: cvId, userId: userId);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an experience
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="experienceId">The unique identifier of the experience to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{experienceId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteAsync(Guid cvId, Guid experienceId)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            await _experienceService.DeleteAsync(experienceId: experienceId, cvId: cvId, userId: userId);
            return NoContent();
        }
    }
}