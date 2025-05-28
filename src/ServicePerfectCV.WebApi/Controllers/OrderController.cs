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
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateRequest request)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("User is not authenticated.");

            request.UserId = Guid.Parse(userIdClaim.Value);
            var orderId = await orderService.CreateAsync(request);
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