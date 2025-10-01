using ServicePerfectCV.Application.DTOs.Section.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class SectionScoreResultService
    {
        private readonly ISectionScoreResultRepository _sectionScoreResultRepository;
        private readonly ICVRepository _cvRepository;

        public SectionScoreResultService(
            ISectionScoreResultRepository sectionScoreResultRepository,
            ICVRepository cvRepository)
        {
            _sectionScoreResultRepository = sectionScoreResultRepository;
            _cvRepository = cvRepository;
        }

        public async Task<CVSectionScoresResponse> GetByCVIdAsync(Guid cvId, Guid userId)
        {
            // Verify the CV exists and belongs to the user
            var cv = await _cvRepository.GetByCVIdAndUserIdAsync(cvId, userId);
            if (cv == null)
            {
                throw new DomainException(CVErrors.CVNotFound);
            }

            var sectionScoreResults = await _sectionScoreResultRepository.GetByCVIdAsync(cvId);
            var sectionScoreResponses = sectionScoreResults.Select(MapToResponse).ToList();

            // Calculate total scores
            var totalScore = sectionScoreResponses.Sum(s => s.SectionScore.TotalScore0To5 * s.SectionScore.Weight0To1);
            var maxPossibleScore = sectionScoreResponses.Sum(s => 5.0 * s.SectionScore.Weight0To1);
            var scorePercentage = maxPossibleScore > 0 ? (totalScore / maxPossibleScore) * 100 : 0;

            return new CVSectionScoresResponse
            {
                CVId = cvId,
                SectionScores = sectionScoreResponses,
                TotalScore = totalScore,
                MaxPossibleScore = maxPossibleScore,
                ScorePercentage = scorePercentage
            };
        }

        private static SectionScoreResultResponse MapToResponse(SectionScoreResult entity)
        {
            return new SectionScoreResultResponse
            {
                Id = entity.Id,
                CVId = entity.CVId,
                SectionType = entity.SectionType,
                SectionScore = entity.SectionScore,
            };
        }
    }
}
