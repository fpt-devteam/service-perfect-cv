using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.DTOs.Order.Requests
{
    public class OrderCreateRequest
    {
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }
}
