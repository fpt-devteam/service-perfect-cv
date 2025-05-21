using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Order.Requests
{
    public class OrderItemRequest
    {
        [Required]

        public Guid ItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}