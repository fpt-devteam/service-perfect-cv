namespace ServicePerfectCV.Application.Constants
{
    public enum Section
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
        public static Section FromString(string section)
        {
            return section.ToLower() switch
            {
                "contact" => Section.Contact,
                "summary" => Section.Summary,
                "skills" => Section.Skills,
                "experience" => Section.Experience,
                "projects" => Section.Projects,
                "education" => Section.Education,
                "certifications" => Section.Certifications,
                _ => throw new ArgumentException($"Unknown section: {section}")
            };
        }

        public static List<Section> AllSections()
        {
            return Enum.GetValues(typeof(Section)).Cast<Section>().ToList();
        }
    }
}
