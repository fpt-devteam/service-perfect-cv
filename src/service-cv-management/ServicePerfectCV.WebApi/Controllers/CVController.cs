using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.CV.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
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

        [Authorize]
        [HttpGet]
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
        public async Task<IActionResult> UpdateAsync(Guid cvId, [FromBody] UpdateCVRequest request)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            var result = await cvService.UpdateAsync(cvId, request, userId);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("{cvId}")]
        public async Task<IActionResult> GetByIdAndUserIdAsync(Guid cvId)
        {
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(nameIdentifier, out var userId))
                throw new DomainException(UserErrors.NotFound);
            var result = await cvService.GetByIdAndUserIdAsync(cvId, userId);
            return Ok(result);
        }
    }
}