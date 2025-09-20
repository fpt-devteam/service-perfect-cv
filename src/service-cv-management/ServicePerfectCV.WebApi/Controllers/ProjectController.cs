using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Project.Requests;
using ServicePerfectCV.Application.DTOs.Project.Responses;
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
    /// Handles project-related operations for CVs
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for creating, reading, updating, and deleting projects associated with a CV
    /// </remarks>
    [ApiController]
    [Authorize]
    [Route("api/cvs/{cvId}/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Creates a new project for a CV
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="request">The project information to create</param>
        /// <returns>The newly created project information</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ProjectResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateAsync(Guid cvId, [FromBody] CreateProjectRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _projectService.CreateAsync(cvId: cvId, userId: userId, request: request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing project
        /// </summary>
        /// <param name="projectId">The unique identifier of the project to update</param>
        /// <param name="request">The updated project information</param>
        /// <returns>The updated project information</returns>
        [HttpPut("{projectId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ProjectResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAsync(Guid projectId, [FromBody] UpdateProjectRequest request)
        {
            var result = await _projectService.UpdateAsync(projectId: projectId, request: request);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves project information for a specific CV.
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="query">The query parameters for pagination and sorting</param>
        /// <remarks>
        /// Query parameters for pagination and sorting.
        /// Default value of <c>limit</c> is 10, <c>offset</c> is 0.
        /// For <c>Sort.StartDate</c>: 0 means ascending (ASC), 1 means descending (DESC).
        /// </remarks>
        /// <returns>The project details associated with the specified CV</returns>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<ProjectResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ListAsync(Guid cvId, [FromQuery] ProjectQuery query)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _projectService.GetByCVIdAsync(cvId: cvId, userId: userId, query: query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves project information by its ID
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="projectId">The unique identifier of the project</param>
        /// <returns>The project details</returns>
        [HttpGet("{projectId}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ProjectResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByIdAsync(Guid cvId, Guid projectId)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _projectService.GetByIdAsync(projectId: projectId, cvId: cvId, userId: userId);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a project
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="projectId">The unique identifier of the project to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{projectId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteAsync(Guid cvId, Guid projectId)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            await _projectService.DeleteAsync(projectId: projectId, cvId: cvId, userId: userId);
            return NoContent();
        }
    }
}