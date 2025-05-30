using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Application.Validators;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/orders")]   
    public class OrdersController(OrderService orderService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateRequest request)
        {
            Guid userIdClaim = Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid id) ? id : throw new DomainException(AuthErrors.UserNotAuthenticated);
            var orderId = await orderService.CreateAsync(userIdClaim, request);
            return Created($"/api/orders/{orderId}", new { orderId });
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(Guid orderId)
        {
            var order = await orderService.GetByIdAsync(orderId);

            return Ok(order);
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] PaginationRequest request)
        {

            var orders = await orderService.ListAsync(request);
            return Ok(orders);
        }

    }
}