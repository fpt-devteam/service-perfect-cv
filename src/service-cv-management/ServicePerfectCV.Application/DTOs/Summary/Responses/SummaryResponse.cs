using System;

namespace ServicePerfectCV.Application.DTOs.Summary.Responses
{
    public class SummaryResponse
    {
        public Guid Id { get; init; }
        public Guid CVId { get; init; }
        public string Context { get; init; } = default!;
    }
}
