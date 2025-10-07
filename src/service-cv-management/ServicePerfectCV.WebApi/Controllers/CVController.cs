using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.CV.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using ServicePerfectCV.Application.DTOs.Section.Responses;
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
    [Authorize]
    [Route("api/cvs")]
    public class CVController(CVService cvService, SectionScoreResultService sectionScoreResultService) : ControllerBase
    {
        /// <summary>
        /// Creates a new CV with job description and optional PDF file
        /// </summary>
        /// <param name="request">CV creation request containing title and job description details</param>
        /// <remarks>
        /// Creates a new CV for the authenticated user. The request must include:
        /// - title: The CV title (required, max 200 characters)
        /// - jobDescription: Object containing job details (required)
        ///   - title: Job title (required)
        ///   - companyName: Company name (required)
        ///   - responsibility: Job responsibilities (required)
        ///   - qualification: Required qualifications (required)
        /// - pdfFile: Optional PDF file upload (max 10MB, PDF only)
        ///
        /// This endpoint accepts multipart/form-data for file uploads.
        ///
        /// Example request (form-data):
        /// - title: "Software Developer CV"
        /// - jobDescription.title: "Senior Software Developer"
        /// - jobDescription.companyName: "Tech Corp"
        /// - jobDescription.responsibility: "Develop and maintain web applications"
        /// - jobDescription.qualification: "Bachelor's degree in Computer Science"
        /// - pdfFile: [PDF file content]
        ///
        /// Example request (JSON without file):
        /// ```json
        /// {
        ///   "title": "Software Developer CV",
        ///   "jobDescription": {
        ///     "title": "Senior Software Developer",
        ///     "companyName": "Tech Corp",
        ///     "responsibility": "Develop and maintain web applications",
        ///     "qualification": "Bachelor's degree in Computer Science"
        ///   }
        /// }
        /// ```
        /// </remarks>
        /// <returns>Created CV with job description details</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(CVResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(413)] // Payload Too Large
        [ProducesResponseType(415)] // Unsupported Media Type
        public async Task<IActionResult> CreateAsync([FromForm] CreateCVRequest request)
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
        /// Example: GET /api/cvs?searchTerm=software&amp;limit=5&amp;offset=0&amp;sort.updatedAt=1
        /// </remarks>
        /// <returns>Paginated list of CVs matching the search criteria</returns>
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
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateCVRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            var result = await cvService.UpdateAsync(cvId: id, request: request, userId: userId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific CV by ID including job description
        /// </summary>
        /// <param name="id">The unique identifier of the CV</param>
        /// <remarks>
        /// Returns the CV details including:
        /// - CV basic information (id, title, content, last edited date)
        /// - Associated job description (title, company name, responsibilities, qualifications)
        ///
        /// Only returns CVs owned by the authenticated user.
        /// </remarks>
        /// <returns>CV details with job description</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CVResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByIdAndUserIdAsync(Guid id)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            var result = await cvService.GetByIdAndUserIdAsync(cvId: id, userId: userId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all section score results for a specific CV with calculated totals
        /// </summary>
        /// <param name="id">The unique identifier of the CV</param>
        /// <remarks>
        /// Returns all section score results associated with the specified CV along with calculated total scores.
        /// Only returns results for CVs owned by the authenticated user.
        ///
        /// The response includes:
        /// - SectionScores: Individual section scores
        /// - TotalScore: Weighted sum of all section scores
        /// - MaxPossibleScore: Maximum possible weighted score
        /// - ScorePercentage: Percentage of total score achieved
        /// </remarks>
        /// <returns>Section score results with calculated totals for the CV</returns>
        [HttpGet("{id}/section-scores")]
        [ProducesResponseType(typeof(CVSectionScoresResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSectionScoreResultsAsync(Guid id)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await sectionScoreResultService.GetByCVIdAsync(cvId: id, userId: userId);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a CV and its associated job description
        /// </summary>
        /// <param name="id">The unique identifier of the CV to delete</param>
        /// <remarks>
        /// This endpoint performs a soft delete on the CV by setting the DeletedAt field to the current timestamp,
        /// and permanently removes the associated job description from the database.
        /// The CV will no longer appear in search results or list operations.
        /// </remarks>
        /// <returns>Success response if CV was deleted</returns>
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