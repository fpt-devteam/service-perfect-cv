using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Item.Requests
{
    public class ItemUpdateRequest
    {
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}