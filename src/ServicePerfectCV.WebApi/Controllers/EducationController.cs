using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Education.Requests;
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
    [Route("api/educations")]
    public class EducationController(EducationService educationService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateEducationRequest request)
        {
            var response = await educationService.CreateAsync(request);
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListAsync([FromQuery] Guid cvId)
        {
            var response = await educationService.ListAsync(cvId);
            return Ok(response);
        }

    }
}