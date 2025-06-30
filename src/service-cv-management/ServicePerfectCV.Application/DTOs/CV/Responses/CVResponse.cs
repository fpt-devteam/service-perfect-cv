using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.CV.Responses
{
    public class CVResponse
    {
        public required Guid CVId { get; init; }
        public required string Title { get; init; } = default!;
        public string FullContent { get; init; } = default!;
        public required DateTime LastEditedAt { get; init; }
    }
}