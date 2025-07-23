using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.CV.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/cvs")]
    public class CVController(CVService cvService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCVRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            var result = await cvService.CreateAsync(request, userId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all CVs for the authenticated user with optional search and pagination
        /// </summary>
        /// <param name="query">Query parameters including search term, pagination, and sorting</param>
        /// <remarks>
        /// Query parameters:
        /// - searchTerm: Search CVs by title (case-insensitive partial match)
        /// - limit: Number of items per page (default: 10)
        /// - offset: Number of items to skip (default: 0)
        /// - sort.updatedAt: Sort by updated date (0 = ascending, 1 = descending)
        /// 
        /// Example: GET /api/cvs?searchTerm=software&limit=5&offset=0&sort.updatedAt=1
        /// </remarks>
        /// <returns>Paginated list of CVs matching the search criteria</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PaginationData<CVResponse>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ListAsync([FromQuery] CVQuery query)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            var result = await cvService.ListAsync(query, userId);
            return Ok(result);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateCVRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            var result = await cvService.UpdateAsync(cvId: id, request: request, userId: userId);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAndUserIdAsync(Guid id)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            var result = await cvService.GetByIdAndUserIdAsync(cvId: id, userId: userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}/full-content")]
        public async Task<IActionResult> GetFullContentAsync(Guid id)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            var result = await cvService.GetFullContentAsync(cvId: id, userId: userId);
            return Ok(result);
        }

        /// <summary>
        /// Soft deletes a CV by setting DeletedAt to current timestamp
        /// </summary>
        /// <param name="id">The unique identifier of the CV to delete</param>
        /// <remarks>
        /// This endpoint performs a soft delete by setting the DeletedAt field to the current timestamp.
        /// The CV will no longer appear in search results or list operations.
        /// </remarks>
        /// <returns>Success response if CV was deleted</returns>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            await cvService.DeleteAsync(cvId: id, userId: userId);

            return Ok(new { message = "CV deleted successfully" });
        }
    }
}