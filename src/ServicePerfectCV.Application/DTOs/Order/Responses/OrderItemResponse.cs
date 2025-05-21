using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Order.Responses
{
    public class OrderItemResponse
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}