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
    Dictionary<string, string> Languages);

public record SectionScoreDto(int score_0_5, List<string> strengths, List<string> current_limitations, List<string> improvement_suggestions);

public record SectionFeedbackMap(
    SectionScoreDto contact,
    SectionScoreDto summary,
    SectionScoreDto skills,
    SectionScoreDto experience,
    SectionScoreDto projects,
    SectionScoreDto education,
    SectionScoreDto certifications);

public record OverallDto(int readiness_score_0_100, string readiness_band, string summary_note);

public record CvAnalysisFinalOutput(OverallDto? overall = null, SectionFeedbackMap? section_feedback = null);