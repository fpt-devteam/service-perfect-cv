using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.EmploymentType.Requests;
using ServicePerfectCV.Application.DTOs.EmploymentType.Responses;
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
    [Route("api/employmentTypes")]
    public class EmploymentTypeController(EmploymentTypeService employmentTypeService) : ControllerBase
    {
        private readonly EmploymentTypeService _employmentTypeService = employmentTypeService;

        /// <summary>
        /// Searches employment types by name for autocomplete suggestions
        /// </summary>
        /// <param name="query">Query parameters including search term and pagination</param>
        /// <returns>List of employment types matching the search criteria</returns>
        [HttpGet("suggestions")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<EmploymentTypeSuggestionResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetSuggestionsAsync([FromQuery] EmploymentTypeQuery query)
        {
            var result = await _employmentTypeService.GetSuggestionsAsync(query);
            return Ok(result);
        }
    }
}
