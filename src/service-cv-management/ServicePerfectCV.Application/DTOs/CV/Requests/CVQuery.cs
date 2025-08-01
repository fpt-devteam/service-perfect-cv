using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Domain.Constants;

namespace ServicePerfectCV.Application.DTOs.CV.Requests
{
    public class CVQuery : PaginationQuery
    {
        public string? SearchTerm { get; set; } = null;
        public CVSort? Sort { get; set; } = null;
    }
    public class CVSort
    {
        public SortOrder? UpdatedAt { get; set; } = null;
    }
}