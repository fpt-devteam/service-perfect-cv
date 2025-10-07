using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;

namespace ServicePerfectCV.Application.Interfaces.AI
{
    public interface ISectionScoreService
    {
        Task<SectionScoreDictionary> ScoreAllSectionsAsync(
           Dictionary<SectionType, string> rubricDictionary,
           Dictionary<SectionType, string> contentDictionary,
           CancellationToken ct);

        Task<SectionScore> ScoreSectionAsync(
         string sectionRubric,
         string sectionContent,
         string sectionName,
         CancellationToken ct);
    }
}
