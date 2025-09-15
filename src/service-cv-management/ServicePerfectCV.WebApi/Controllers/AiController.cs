using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Services;

namespace ServicePerfectCV.WebApi.Controllers;

[ApiController]
[Route("api/ai")]
public sealed class AIController : ControllerBase
{
    private readonly IAIOrchestrator _orchestrator;
    private readonly IJobStore<CvAnalysisFinalOutput> _jobStore;
    private readonly IJobProcessingService _jobProcessingService;

    public AIController(
        IAIOrchestrator orchestrator,
        IJobStore<CvAnalysisFinalOutput> jobStore,
        IJobProcessingService jobProcessingService)
    {
        _orchestrator = orchestrator;
        _jobStore = jobStore;
        _jobProcessingService = jobProcessingService;
    }


    [HttpGet("cv/analysis/{jobId}")]
    public IActionResult GetAnalResult(string jobId)
    {
        var job = _jobStore.GetJob(jobId);
        if (job == null)
        {
            return NotFound(new { message = "Job not found" });
        }

        return Ok(new
        {
            jobId = job.JobId,
            status = job.Status.ToString(),
            result = job.Result,
            error = job.Error,
            createdAt = job.CreatedAt,
            completedAt = job.CompletedAt
        });
    }

    private JobDescription CreateWorldQuantJobDescription()
    {
        return new JobDescription(
            "Software Engineer Intern",
            "intern",
            new List<string>{
                    "A degree in a quantitative or technical discipline; excellent academic performance. Academic achievement such as International/National Olympiad medal is a plus",
                    "Strong programming skills in one or more object-oriented programming language",
                    "In depth understanding of Unix/Linux",
                    "Excellent problem solving and analytical skills with strong attention to detail",
                    "Mature, thoughtful, with the ability to operate in a collaborative, team-oriented culture",
                    "Self-motivated and take initiative; ask thoughtful questions to achieve desired results"
            });
    }

    private CvEntity CreateSoftwareCv()
    {
        return new CvEntity(
            Contact: new ContactVO(
                Name: "TRAN NHAT THANG",
                Position: "Software Engineer Intern",
                Phone: "+84784904003",
                Email: "nhatthang270404@gmail.com",
                Linkedin: "linkedin.com/in/nhatthang27",
                Github: "github.com/Nhatthang27",
                Location: "Ho Chi Minh City, Vietnam"
            ),
            CareerObjective: "A third-year Software Engineering student with a strong foundation in algorithms, data structures, and object-oriented programming. Passionate about software development and high-performance systems, seeking an internship opportunity at WorldQuant to contribute to cutting-edge software solutions for automated investment processes.",
            TechnicalSkills: new Dictionary<string, string>
            {
                ["algorithms"] = "Strong expertise in algorithms and competitive programming (Codeforces 1662, LeetCode 1805)",
                ["backend"] = ".NET Core, ASP.NET Core, Entity Framework Core, RESTful API, SQL Server",
                ["frontend"] = "HTML/CSS, ReactJS",
                ["architecture"] = "Microservices (basic), 3-Tier Architecture, Clean Architecture",
                ["devops"] = "Basic knowledge of Docker, CI/CD",
                ["programming"] = "C#, C++, JavaScript"
            },
            Achievements: new List<string>
            {
                    "ICPC Asia Hanoi Regional 2024: Rank 48th",
                    "ICPC Vietnam Central 2023: Second Prize",
                    "National Excellent Student Contest in Informatics 2022: Consolation Prize",
                    "Da Nang Code League 2024: Rank 53rd",
                    "VNG Code Tour 2024: Rank 66th - Final Round",
                    "Top 100 Outstanding Software Engineering Students of Fall23 Semester",
                    "Top 5 Outstanding Software Engineering Students of Spring24 Semester"
            },
            Experience: new List<ExperienceItem>
            {
                    new ExperienceItem(
                        Company: "FPT Software",
                        Position: "On-the-Job Training – Japanese BrSE with .NET Program",
                        Details: new List<string>
                        {
                            "Gained foundational knowledge of C# and .NET, covering core concepts and best practices.",
                            "Collaborated on group exercises and projects, applying technical skills to solve real-world problems.",
                            "Developed problem-solving abilities and teamwork skills in a structured, professional environment."
                        }
                    )
            },
            Projects: new List<ProjectItem>
            {
                    new ProjectItem(
                        Name: "Music Player App",
                        Description: "A desktop music player supporting audio playback, playlist creation, history, and shuffle.",
                        Technologies: new List<string> { "WPF", "Entity Framework Core", "3-Tier Architecture" },
                        Contributions: new List<string>
                        {
                            "Designed and developed UI with XAML, optimizing UX.",
                            "Designed database and integrated Entity Framework.",
                            "Implemented features like play, pause, shuffle, search, and queue."
                        }
                    ),
                    new ProjectItem(
                        Name: "Koi Auction",
                        Description: "An online auction platform for buying and selling Koi fish with real-time bidding and integrated e-wallet.",
                        Technologies: new List<string> { "ASP.NET Core", "Entity Framework Core", "SignalR", "SQL Server", "ReactJS", "Node.js", "Microservices" },
                        Contributions: new List<string>
                        {
                            "Designed and developed frontend with ReactJS.",
                            "Built backend APIs for auction management and scheduling.",
                            "Integrated SignalR for real-time bid synchronization.",
                            "Developed auction scheduling logic for automatic start/end."
                        }
                    )
            },
            Education: new List<EducationItem>
            {
                    new EducationItem(
                        School: "Le Quy Don High School for the Gifted",
                        Major: "",
                        Years: "2019 - 2022",
                        Gpa: ""
                    ),
                    new EducationItem(
                        School: "FPT University",
                        Major: "Software Engineering",
                        Years: "2022 - now",
                        Gpa: "3.4"
                    )
            },
            Languages: new Dictionary<string, string>
            {
                ["english"] = "Proficient – able to read, understand technical documents, and communicate effectively.",
                ["japanese"] = "Basic (JLPT N5) – understand simple sentences and basic conversation."
            }
        );
    }

    private CvEntity CreateMarketingCv()
    {    // Hardcoded CV
        return new CvEntity(
            Contact: new ContactVO(
                Name: "NGUYEN THI ANH",
                Position: "Marketing Intern",
                Phone: "+84901234567",
                Email: "anh.nguyen@example.com",
                Linkedin: "linkedin.com/in/nguyenthianh",
                Github: "github.com/nguyenthianh", // portfolio/source files for class projects
                Location: "Ho Chi Minh City, Vietnam"
            ),
            CareerObjective: "A final-year Marketing student passionate about brand building and consumer insights. Seeking a Marketing Intern role at Vinamilk to support market research, digital campaigns, and brand activation for leading dairy products while learning best practices in FMCG marketing.",
            TechnicalSkills: new Dictionary<string, string>
            {
                ["marketing"] = "Brand basics, 4P/4C, STP, consumer journey, AIDA, IMC planning",
                ["digital_marketing"] = "Content planning, basic Meta Ads & TikTok Ads, social listening, community management",
                ["analytics"] = "Survey design, basic statistics, cohort/retention basics, KPI tracking (reach, CTR, engagement rate)",
                ["tools"] = "Excel/Google Sheets (pivot, VLOOKUP), PowerPoint, Google Forms, Canva, Google Analytics (basic)",
                ["research"] = "Desk research, competitor benchmarking, shopper/consumer insight synthesis",
                ["soft_skills"] = "Communication, teamwork, time management, attention to detail"
            },
            Achievements: new List<string>
            {
                "Top 10 University Marketing Case Competition (2024)",
                "Dean’s List – Business Administration (2023)",
                "Hult Prize Campus Finalist (2024)",
                "Organizing Committee – University Marketing Club (Content Lead)",
                "Volunteer – Brand activation events at campus (1,500+ students reached)",
                "Completed Google Analytics for Beginners certification"
            },
            Experience: new List<ExperienceItem>
            {
                new ExperienceItem(
                    Company: "University Communications Office",
                    Position: "Marketing Collaborator",
                    Details: new List<string>
                    {
                        "Planned and produced weekly content for Facebook fanpage; average engagement rate 6–8%.",
                        "Coordinated 3 on-campus events (200–500 attendees) including logistics, MC scripts, and media coverage.",
                        "Compiled monthly performance reports (reach, ER, top posts) and proposed optimizations."
                    }
                )
            },
            Projects: new List<ProjectItem>
            {
                new ProjectItem(
                    Name: "Vinamilk Flavored Milk – Campus Market Research",
                    Description: "A course project to explore flavor preferences, price sensitivity, and purchase triggers among students for flavored milk.",
                    Technologies: new List<string> { "Google Forms", "Excel", "PowerPoint", "Canva" },
                    Contributions: new List<string>
                    {
                        "Designed questionnaire (n=310) and cleaned data; built pivot tables and charts for insights.",
                        "Segmented respondents by usage frequency and flavor preference; identified top 3 attributes.",
                        "Presented recommendations on flavor portfolio and bundle promotions for student channels."
                    }
                ),
                new ProjectItem(
                    Name: "Back-to-School Social Campaign",
                    Description: "Designed a 4-week social media mini-campaign concept for a dairy snack brand targeting freshmen.",
                    Technologies: new List<string> { "Content Calendar", "Canva", "Meta Ads (basic)", "Google Sheets" },
                    Contributions: new List<string>
                    {
                        "Built content calendar (12 posts + 2 short videos) focusing on study tips and healthy snacking.",
                        "Drafted key visuals and copy; proposed KOL seeding with micro-influencers on TikTok.",
                        "Estimated media budget and KPIs (reach, ER, CTR) with weekly optimization plan."
                    }
                )
            },
            Education: new List<EducationItem>
            {
                new EducationItem(
                    School: "Le Quy Don High School for the Gifted",
                    Major: "",
                    Years: "2019 - 2022",
                    Gpa: ""
                ),
                new EducationItem(
                    School: "FPT University",
                    Major: "Marketing (Business Administration)",
                    Years: "2022 - now",
                    Gpa: "3.5"
                )
            },
            Languages: new Dictionary<string, string>
            {
                ["vietnamese"] = "Native – professional communication in academic and work settings.",
                ["english"] = "Fluent – able to read industry reports, write marketing materials, and present."
            }
        );

    }
    [HttpPost("cv/analysis")]
    public IActionResult AnalyzeCv(CancellationToken ct)
    {
        // Hardcoded Job Description
        var jd = CreateWorldQuantJobDescription();

        // Hardcoded CV
        // var cv = CreateSoftwareCv();
        var cv = CreateMarketingCv();

        var jobId = _jobStore.CreateJob();

        _ = Task.Run(async () =>
        {
            try
            {
                await _jobProcessingService.ProcessCvAnalysisJobAsync(
                    jobId: jobId,
                    cv: cv,
                    jd: jd,
                    ct: ct
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        });


        return Ok(new { jobId });
    }
}
