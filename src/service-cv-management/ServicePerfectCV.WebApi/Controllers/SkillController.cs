using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Skill.Requests;
using ServicePerfectCV.Application.DTOs.Skill.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    /// <summary>
    /// Handles skill-related operations for CVs
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for creating, reading, updating, and deleting skills associated with a CV
    /// </remarks>
    [ApiController]
    [Authorize]
    [Route("api/cvs/{cvId}/skills")]
    public class SkillController : ControllerBase
    {
        private readonly SkillService _skillService;

        public SkillController(SkillService skillService)
        {
            _skillService = skillService;
        }

        /// <summary>
        /// Creates a new skill for a CV
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="request">The skill information to create</param>
        /// <returns>The newly created skill information</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SkillResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateAsync(Guid cvId, [FromBody] CreateSkillRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _skillService.CreateAsync(cvId: cvId, userId: userId, request: request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing skill
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="skillId">The unique identifier of the skill to update</param>
        /// <param name="request">The updated skill information</param>
        /// <returns>The updated skill information</returns>
        [HttpPut("{skillId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SkillResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid cvId, Guid skillId, [FromBody] UpdateSkillRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _skillService.UpdateAsync(skillId: skillId, cvId: cvId, userId: userId, request: request);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves skill information for a specific CV.
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="query">The query parameters for pagination and sorting</param>
        /// <remarks>
        /// Query parameters for pagination and sorting.
        /// Default value of <c>limit</c> is 10, <c>offset</c> is 0.
        /// For <c>Sort.Category</c>: 0 means ascending (ASC), 1 means descending (DESC).
        /// </remarks>
        /// <returns>The skill details associated with the specified CV</returns>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<SkillResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ListAsync(Guid cvId, [FromQuery] SkillQuery query)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _skillService.GetByCVIdAsync(cvId: cvId, userId: userId, query: query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves skill information by its ID
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="skillId">The unique identifier of the skill</param>
        /// <returns>The skill details</returns>
        [HttpGet("{skillId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SkillResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByIdAsync(Guid cvId, Guid skillId)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _skillService.GetByIdAsync(skillId: skillId, cvId: cvId, userId: userId);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a skill
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="skillId">The unique identifier of the skill to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{skillId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteAsync(Guid cvId, Guid skillId)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            await _skillService.DeleteAsync(skillId: skillId, cvId: cvId, userId: userId);
            return NoContent();
        }
    }
}