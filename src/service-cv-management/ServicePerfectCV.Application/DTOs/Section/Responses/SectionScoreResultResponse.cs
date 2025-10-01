using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace ServicePerfectCV.Application.DTOs.Section.Responses
{
    public class SectionScoreResultResponse
    {
        public Guid Id { get; set; }
        public Guid CVId { get; set; }
        public SectionType SectionType { get; set; }
        public SectionScore SectionScore { get; set; } = default!;
    }

    public class CVSectionScoresResponse
    {
        public Guid CVId { get; set; }
        public List<SectionScoreResultResponse> SectionScores { get; set; } = new();
        public double TotalScore { get; set; }
        public double MaxPossibleScore { get; set; }
        public double ScorePercentage { get; set; }
    }
}
