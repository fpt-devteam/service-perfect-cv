using ServicePerfectCV.Application.DTOs.Education.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.CV.Responses
{
    public class CVResponse
    {
        public Guid UserId { get; init; }
        public string Title { get; init; } = default!;
        public JobDetailDto? JobDetail { get; init; }
        public IEnumerable<EducationResponse> Educations { get; init; } = null!;

    }
}