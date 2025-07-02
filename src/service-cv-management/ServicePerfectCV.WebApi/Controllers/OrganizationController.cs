using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Organization.Requests;
using ServicePerfectCV.Application.DTOs.Organization.Responses;
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
    [Route("api/organizations")]
    public class OrganizationController(OrganizationService organizationService) : ControllerBase
    {
        

        /// <summary>
        /// Searches organizations by name for autocomplete suggestions
        /// </summary>
        /// <param name="query">Query parameters including search term and pagination</param>
        /// <returns>List of organizations matching the search criteria</returns>
        [HttpGet("suggestions")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<OrganizationSuggestionResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetSuggestionsAsync([FromQuery] OrganizationQuery query)
        {
            var result = await organizationService.GetSuggestionsAsync(query);
            return Ok(result);
        }
    }
}
