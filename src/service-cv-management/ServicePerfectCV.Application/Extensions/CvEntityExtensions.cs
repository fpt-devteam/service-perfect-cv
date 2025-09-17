using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.Interfaces;

namespace ServicePerfectCV.Application.Extensions
{
    public static class CvEntityExtensions
    {
        public static Dictionary<Section, string> ToContentDictionary(this CvEntity cv, IJsonHelper jsonHelper)
        {
            return new Dictionary<Section, string>
            {
                { Section.Contact, jsonHelper.Serialize(cv.Contact)},
                { Section.Skills, jsonHelper.Serialize(cv.TechnicalSkills) },
                { Section.Experience, jsonHelper.Serialize(cv.Experience) },
                { Section.Projects, jsonHelper.Serialize(cv.Projects) },
                { Section.Education, jsonHelper.Serialize(cv.Education) },
                { Section.Certifications, jsonHelper.Serialize(cv.Achievements) }
            };
        }
    }
}