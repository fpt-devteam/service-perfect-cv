using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Degree.Requests;
using ServicePerfectCV.Application.DTOs.Degree.Responses;
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
    [Route("api/degrees")]
    public class DegreeController(DegreeService degreeService) : ControllerBase
    {

        /// <summary>
        /// Searches degrees by name or code for autocomplete suggestions
        /// </summary>
        /// <param name="query">Query parameters including search term and pagination</param>
        /// <returns>List of degrees matching the search criteria</returns>
        [HttpGet("suggestions")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<DegreeSuggestionResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetSuggestionsAsync([FromQuery] DegreeQuery query)
        {
            var result = await degreeService.GetSuggestionsAsync(query);
            return Ok(result);
        }
    }
}
