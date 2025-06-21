using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.Pagination.Requests
{
    public class PaginationQuery
    {
        [Range(1, int.MaxValue, ErrorMessage = "Limit must be at least 1.")]
        public int Limit { get; set; } = 10;
        [Range(0, int.MaxValue, ErrorMessage = "Offset must be at least 0.")]
        public int Offset { get; set; } = 0;

    }
}