using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Order.Requests
{
    public class OrderCreateRequest
    {
        public IEnumerable<OrderItemRequest> Items { get; set; } = default!;
    }
}