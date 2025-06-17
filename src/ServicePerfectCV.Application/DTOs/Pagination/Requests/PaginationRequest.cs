using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Pagination.Requests
{
    public class PaginationRequest
    {
        [MinLength(1, ErrorMessage = "Limit must be at least 1.")]
        public int Limit { get; set; }
        [MinLength(0, ErrorMessage = "Offset must be at least 0.")]
        public int Offset { get; set; }

    }
}