using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Enums;

namespace ServicePerfectCV.Application.Extensions
{
    public static class CvEntityExtensions
    {
        public static Dictionary<SectionType, string> ToContentDictionary(this CvEntity cv, IJsonHelper jsonHelper)
        {
            return new Dictionary<SectionType, string>
            {
                { SectionType.Contact, jsonHelper.Serialize(cv.Contact)},
                { SectionType.Skills, jsonHelper.Serialize(cv.TechnicalSkills) },
                { SectionType.Experience, jsonHelper.Serialize(cv.Experience) },
                { SectionType.Projects, jsonHelper.Serialize(cv.Projects) },
                { SectionType.Education, jsonHelper.Serialize(cv.Education) },
                { SectionType.Certifications, jsonHelper.Serialize(cv.Achievements) }
            };
        }
    }
}