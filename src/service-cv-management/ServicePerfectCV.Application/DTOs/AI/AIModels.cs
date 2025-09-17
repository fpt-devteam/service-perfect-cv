using ServicePerfectCV.Application.Constants;

namespace ServicePerfectCV.Application.DTOs.AI;

public record JobDescription(string Title, string Level, List<string> Requirements);

public record ContactVO(string Name, string Position, string Phone, string Email, string Linkedin, string Github, string Location);

public record ExperienceItem(string Company, string Position, List<string> Details);

public record ProjectItem(string Name, string Description, List<string> Technologies, List<string> Contributions);

public record EducationItem(string School, string Major, string Years, string Gpa);

public record CvEntity(
    ContactVO Contact,
    string CareerObjective,
    Dictionary<string, string> TechnicalSkills,
    List<string> Achievements,
    List<ExperienceItem> Experience,
    List<ProjectItem> Projects,
    List<EducationItem> Education,
    Dictionary<string, string> Languages
);

public record OverallDto(
    int readiness_score_0_100,
    string readiness_band,
    string summary_note,
    Dictionary<string, double> section_weighted_scores
);

public record CvAnalysisFinalOutput(SectionScoreDictionary? sectionScores = null);