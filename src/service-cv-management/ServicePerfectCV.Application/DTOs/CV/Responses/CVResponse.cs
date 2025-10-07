using ServicePerfectCV.Application.DTOs.CV;
using ServicePerfectCV.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.DTOs.CV.Responses
{
    public class CVResponse
    {
        public required Guid CVId { get; init; }
        public string? Title { get; init; }
        public JobDescriptionResponse? JobDescription { get; init; }
        public CVContent? Content { get; init; }
        public DateTimeOffset? LastEditedAt { get; init; }
        public bool IsStructuredDone { get; init; }
    }


    public class JobDescriptionResponse
    {
        public Guid Id { get; init; }
        public Guid CVId { get; init; }
        public string Title { get; init; } = null!;
        public string CompanyName { get; init; } = null!;
        public string Responsibility { get; init; } = null!;
        public string Qualification { get; init; } = null!;
        public SectionRubricDictionary? SectionRubrics { get; init; }
    }

}