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
        public string CvFullContent { get; init; } = default!;
    }
}