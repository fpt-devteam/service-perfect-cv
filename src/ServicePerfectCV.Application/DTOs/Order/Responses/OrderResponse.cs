using ServicePerfectCV.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Order.Responses
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; } = default!;
        public DateTime OrderDate { get; set; }
        public IEnumerable<OrderItemResponse> Items { get; set; } = default!;
    }
}