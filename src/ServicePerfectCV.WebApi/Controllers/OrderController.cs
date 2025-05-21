using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.Order.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Request;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Application.Validators;
using ServicePerfectCV.Domain.Entities;

namespace ServicePerfectCV.WebApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateRequest request)
        {

            var orderId = await _orderService.CreateAsync(request);
            return Created($"/api/orders/{orderId}", new { orderId });
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(Guid orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null)
                return NotFound(new { Message = "Order not found." });

            return Ok(order);
        }

        [HttpPost("list")]
        public async Task<IActionResult> ListAll([FromBody] PaginationRequest request)
        {
            if (request == null || request.Limit <= 0)
                return BadRequest(new { Message = "Invalid pagination request." });

            var orders = await _orderService.ListAllAsync(request);
            return Ok(orders);
        }

    }
}
