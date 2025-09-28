namespace ServicePerfectCV.Domain.Enums
{
    public enum SectionType
    {
        Contact,
        Summary,
        Skills,
        Experience,
        Projects,
        Education,
        Certifications
    }

    public static class SectionTypeExtensions
    {
        public static SectionType FromString(string section)
        {
            return section.ToLower() switch
            {
                "contact" => SectionType.Contact,
                "summary" => SectionType.Summary,
                "skills" => SectionType.Skills,
                "experience" => SectionType.Experience,
                "projects" => SectionType.Projects,
                "education" => SectionType.Education,
                "certifications" => SectionType.Certifications,
                _ => throw new ArgumentException($"Unknown section: {section}")
            };
        }
    }
}

