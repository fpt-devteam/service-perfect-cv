using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.JobTitle.Requests;
using ServicePerfectCV.Application.DTOs.JobTitle.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/job-titles")]
    public class JobTitleController : ControllerBase
    {
        private readonly JobTitleService _jobTitleService;

        public JobTitleController(JobTitleService jobTitleService)
        {
            _jobTitleService = jobTitleService;
        }

        /// <summary>
        /// Searches job titles by name for autocomplete suggestions
        /// </summary>
        /// <param name="query">Query parameters including search term and pagination</param>
        /// <returns>List of job titles matching the search criteria</returns>
        [HttpGet("suggestions")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<JobTitleSuggestionResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetSuggestionsAsync([FromQuery] JobTitleQuery query)
        {
            var result = await _jobTitleService.GetSuggestionsAsync(query);
            return Ok(result);
        }
    }
}