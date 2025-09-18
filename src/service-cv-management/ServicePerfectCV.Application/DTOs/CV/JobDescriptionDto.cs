using System;

namespace ServicePerfectCV.Application.DTOs.CV
{
    public class JobDescriptionDto
    {
        public Guid Id { get; init; }
        public Guid CVId { get; init; }
        public string Title { get; init; } = null!;
        public string CompanyName { get; init; } = null!;
        public string Responsibility { get; init; } = null!;
        public string Qualification { get; init; } = null!;
    }
}