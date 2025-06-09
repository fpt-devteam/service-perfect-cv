using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.CV.Responses
{
    public class JobDetailResponse
    {
        public string JobTitle { get; init; } = default!;
        public string CompanyName { get; init; } = default!;
        public string Description { get; init; } = default!;
    }
}