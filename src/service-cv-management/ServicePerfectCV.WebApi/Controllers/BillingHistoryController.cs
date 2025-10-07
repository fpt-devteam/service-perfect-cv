using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Billing.Requests;
using ServicePerfectCV.Application.DTOs.Billing.Responses;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/billing-histories")]
    [Produces("application/json")]
    public class BillingHistoryController : ControllerBase
    {
        private readonly BillingHistoryService _billingHistoryService;

        public BillingHistoryController(BillingHistoryService billingHistoryService)
        {
            _billingHistoryService = billingHistoryService;
        }

        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<BillingHistoryResponse>), 200)]
        public async Task<ActionResult<IEnumerable<BillingHistoryResponse>>> GetByUserId([FromRoute] Guid userId)
        {
            var items = await _billingHistoryService.GetByUserIdAsync(userId);
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BillingHistoryResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<BillingHistoryResponse>> GetById([FromRoute] Guid id)
        {
            var item = await _billingHistoryService.GetByIdAsync(id)
                ?? throw new DomainException(BillingErrors.NotFound);

            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BillingHistoryResponse), 201)]
        public async Task<ActionResult<BillingHistoryResponse>> Create([FromBody] CreateBillingHistoryRequest request)
        {
            var created = await _billingHistoryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
