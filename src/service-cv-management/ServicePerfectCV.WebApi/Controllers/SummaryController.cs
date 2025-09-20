using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Summary.Requests;
using ServicePerfectCV.Application.DTOs.Summary.Responses;
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
    /// <summary>
    /// Handles summary-related operations for CVs
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for creating, reading, and updating summary information associated with a CV
    /// </remarks>
    [ApiController]
    [Authorize]
    [Route("api/cvs/{cvId}/summary")]
    public class SummaryController : ControllerBase
    {
        private readonly SummaryService _summaryService;

        public SummaryController(SummaryService summaryService)
        {
            _summaryService = summaryService;
        }

        /// <summary>
        /// Creates or updates summary information
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <param name="request">The summary information to create or update</param>
        /// <returns>The newly created or updated summary information</returns>
        /// <remarks>
        /// This endpoint handles both creation and update operations for summary information.
        /// If the summary already exists (based on CV ID), it will be updated; otherwise, a new summary will be created.
        /// </remarks>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SummaryResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpsertAsync(Guid cvId, [FromBody] UpsertSummaryRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);

            var result = await _summaryService.UpsertAsync(cvId: cvId, userId: userId, request: request);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves summary information for a specific CV
        /// </summary>
        /// <param name="cvId">The unique identifier of the CV</param>
        /// <returns>The summary details associated with the specified CV</returns>
        /// <remarks>
        /// This endpoint returns summary information linked to the provided CV ID.
        /// If no summary information is found, a NotFound response is returned.
        /// </remarks>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SummaryResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByCVIdAsync(Guid cvId)
        {
            var result = await _summaryService.GetByCVIdAsync(cvId);
            return Ok(result);
        }
    }
}