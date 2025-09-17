using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Infrastructure.Helpers;

namespace ServicePerfectCV.Infrastructure.Extensions
{
    public static class CvEntityExtensions
    {
        public static Dictionary<Section, string> ToContentDictionary(this CvEntity cv)
        {
            return new Dictionary<Section, string>
            {
                { Section.Contact, JsonHelper.Serialize(cv.Contact)},
                { Section.Skills, JsonHelper.Serialize(cv.TechnicalSkills) },
                { Section.Experience, JsonHelper.Serialize(cv.Experience) },
                { Section.Projects, JsonHelper.Serialize(cv.Projects) },
                { Section.Education, JsonHelper.Serialize(cv.Education) },
                { Section.Certifications, JsonHelper.Serialize(cv.Achievements) }
            };
        }
    }
}
