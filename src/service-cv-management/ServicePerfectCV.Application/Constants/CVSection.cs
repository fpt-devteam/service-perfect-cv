namespace ServicePerfectCV.Application.Constants
{
    public enum CVSection
    {
        Contact,
        Summary,
        Skills,
        Experience,
        Projects,
        Education,
        Certifications
    }
    public static class CVSectionExtensions
    {
        public static CVSection FromString(string section)
        {
            return section.ToLower() switch
            {
                "contact" => CVSection.Contact,
                "summary" => CVSection.Summary,
                "skills" => CVSection.Skills,
                "experience" => CVSection.Experience,
                "projects" => CVSection.Projects,
                "education" => CVSection.Education,
                "certifications" => CVSection.Certifications,
                _ => throw new ArgumentException($"Unknown section: {section}")
            };
        }

        public static List<CVSection> AllSections()
        {
            return Enum.GetValues(typeof(CVSection)).Cast<CVSection>().ToList();
        }
    }
}
