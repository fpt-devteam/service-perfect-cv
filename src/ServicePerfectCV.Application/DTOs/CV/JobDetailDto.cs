using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.CV
{
    public class JobDetailDto
    {
        public string JobTitle { get; init; } = null!;
        public string CompanyName { get; init; } = null!;
        public string Description { get; init; } = null!;
    }
}