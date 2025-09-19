using Microsoft.AspNetCore.Mvc;
using ServicePerfectCV.Application.Constants;
using ServicePerfectCV.Application.DTOs.AI;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Services;
using ServicePerfectCV.Infrastructure.Constants;

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
            await _jobProcessingService.ProcessScoreCvSectionJobAsync(
                jobId: jobId,
                cv: cv,
                sectionRubric: MockRubricDictionary(),
                ct: ct
            );
        });


        return Ok(new { jobId });
    }


    private SectionRubricDictionary MockRubricDictionary() => new()
        {
            { Section.Contact, MockContactRubric() },
            { Section.Skills, MockSkillRubric() },
            { Section.Education, MockEducationRubric() },
            { Section.Experience, MockExperienceRubric() },
            { Section.Projects, MockProjectsRubric() },
            { Section.Summary, MockSummaryRubric() }
        };

    private SectionRubric MockSkillRubric() => new()
    {
        Weight0To1 = 0.25,
        Criteria = new List<SectionRubricCriteria>
    {
        new SectionRubricCriteria
        {
            Id = "skills.oop_programming",
            Criterion = "Object-Oriented Programming (OOP) Proficiency",
            Description = "Evaluate the depth and breadth of programming skills in one or more object-oriented programming languages, including understanding of OOP concepts (encapsulation, inheritance, polymorphism, abstraction) demonstrated through projects or coursework.",
            Weight0To1 = 0.4,
            Scoring = new ScoringScale
            {
                Zero  = "No mention of OOP languages or minimal exposure without practical application. Basic syntax knowledge at best.",
                One   = "Limited exposure to an OOP language, only basic syntax, no clear understanding of core OOP principles.",
                Two   = "Familiarity with one OOP language; can write simple programs but struggles with applying core OOP concepts.",
                Three = "Solid programming skills in one OOP language (e.g., Python, Java, C++). Can implement basic OOP principles in projects, demonstrating understanding of data structures and algorithms.",
                Four  = "Strong programming skills in one or more OOP languages. Demonstrates good understanding and application of OOP principles (e.g., design patterns, class hierarchy) in projects. Able to write clean, efficient, and well-structured code.",
                Five  = "Excellent programming skills in multiple OOP languages or deep expertise in one. Demonstrates advanced application of OOP principles, complex data structures, and algorithms in significant projects. Code is robust, scalable, and follows best practices, showing strong problem-solving capabilities."
            }
        },
        new SectionRubricCriteria
        {
            Id = "skills.unix_linux",
            Criterion = "Unix/Linux Understanding",
            Description = "Assess the candidate's understanding of Unix/Linux environments, including command-line usage, scripting, file systems, and basic system administration concepts, as 'in-depth understanding' is required.",
            Weight0To1 = 0.3,
            Scoring = new ScoringScale
            {
                Zero  = "No mention of Unix/Linux experience, or states no familiarity.",
                One   = "Minimal exposure to Unix/Linux, perhaps through basic commands in a classroom setting, no practical application.",
                Two   = "Basic familiarity with common Unix/Linux commands (e.g., `ls`, `cd`, `grep`), can navigate the file system.",
                Three = "Good understanding of Unix/Linux. Proficient with common commands, shell scripting (e.g., Bash), and understands basic file permissions and process management. Applied in coursework or personal projects.",
                Four  = "Strong, in-depth understanding of Unix/Linux. Comfortable with advanced command-line tools, debugging utilities, scripting for automation, and understands core OS concepts (processes, memory, I/O). Demonstrated in significant projects or previous intern roles.",
                Five  = "Exceptional, in-depth understanding of Unix/Linux. Can troubleshoot complex issues, write advanced shell scripts, manipulate system processes, and understands system architecture deeply. Potentially contributed to Linux-based projects or managed a personal server environment effectively."
            }
        },
        new SectionRubricCriteria
        {
            Id = "skills.problem_solving_analytical",
            Criterion = "Problem Solving & Analytical Skills",
            Description = "Evaluate the candidate's inherent ability to break down complex problems, think analytically, and pay strong attention to detail, as evidenced by academic performance, project descriptions, or contest participation.",
            Weight0To1 = 0.3,
            Scoring = new ScoringScale
            {
                Zero  = "No indication of problem-solving abilities or attention to detail; CV contains errors or inconsistencies.",
                One   = "Limited evidence of analytical thinking; projects are trivial or poorly explained. CV has minor errors.",
                Two   = "Basic problem-solving skills evident, perhaps through simple coursework projects. Some attention to detail but not consistently applied.",
                Three = "Good problem-solving and analytical skills, demonstrated through relevant coursework or projects. Shows reasonable attention to detail in CV and descriptions.",
                Four  = "Excellent problem-solving and analytical skills, applied to challenging academic problems or complex projects. Strong attention to detail, evidenced by error-free and well-structured CV and project descriptions. Potential participation in competitive programming.",
                Five  = "Outstanding problem-solving and analytical abilities, evidenced by exceptional academic performance, participation/medals in competitive programming (e.g., Olympiads), or tackling highly complex problems in projects. Demonstrates meticulous attention to detail in all aspects of their work and presentation."
            }
        }
    }
    };

    private SectionRubric MockContactRubric() => new()
    {
        Weight0To1 = 0.10,
        Criteria = new List<SectionRubricCriteria>
    {
        new SectionRubricCriteria
        {
            Id = "contact.basic",
            Criterion = "Professional and Complete Contact Information",
            Description = "Clear, professional email/phone, plus LinkedIn and/or GitHub.",
            Weight0To1 = 1.0,
            Scoring = new ScoringScale
            {
                Zero  = "Missing or unprofessional contact info.",
                One   = "Only email/phone provided, incomplete or unprofessional.",
                Two   = "Email and phone are present, but missing LinkedIn/GitHub.",
                Three = "Professional email/phone plus either LinkedIn or GitHub.",
                Four  = "Professional email/phone plus both LinkedIn and GitHub.",
                Five  = "Complete, professional contact info with extra links (e.g., portfolio, website) presented clearly."
            }
        }
    }
    };

    private SectionRubric MockEducationRubric() => new()
    {
        Weight0To1 = 0.15,
        Criteria = new List<SectionRubricCriteria>
    {
        new SectionRubricCriteria
        {
            Id = "edu.degree",
            Criterion = "Relevant Degree and Major",
            Description = "Degree or major in quantitative/technical discipline.",
            Weight0To1 = 0.6,
            Scoring = new ScoringScale
            {
                Zero  = "No degree or irrelevant major.",
                One   = "Unrelated major with minimal technical coursework.",
                Two   = "Currently studying in a relevant field but unclear depth.",
                Three = "Pursuing or completed relevant degree with average standing.",
                Four  = "Completed relevant degree with strong performance.",
                Five  = "Completed relevant degree with exceptional performance or advanced study (graduate-level)."
            }
        },
        new SectionRubricCriteria
        {
            Id = "edu.academic",
            Criterion = "Academic Performance & Achievements",
            Description = "GPA, scholarships, Olympiad or competition awards.",
            Weight0To1 = 0.4,
            Scoring = new ScoringScale
            {
                Zero  = "No academic performance mentioned.",
                One   = "Low GPA or weak academic record.",
                Two   = "Average GPA without distinctions.",
                Three = "Good GPA or small awards/scholarships.",
                Four  = "High GPA and notable academic recognition.",
                Five  = "Outstanding GPA plus major awards (national/international Olympiad or scholarships)."
            }
        }
    }
    };

    private SectionRubric MockExperienceRubric() => new()
    {
        Weight0To1 = 0.25,
        Criteria = new List<SectionRubricCriteria>
    {
        new SectionRubricCriteria
        {
            Id = "exp.problem_solving",
            Criterion = "Problem Solving & Analytical Skills",
            Description = "Ability to solve algorithmic or real-world problems effectively.",
            Weight0To1 = 0.5,
            Scoring = new ScoringScale
            {
                Zero  = "No evidence of problem-solving ability.",
                One   = "Minimal: basic coursework or trivial tasks.",
                Two   = "Some problem-solving in small projects or coursework.",
                Three = "Good: applied problem-solving in internships or personal projects.",
                Four  = "Strong: coding contests, internships, or complex project work.",
                Five  = "Outstanding: national/international competitions (ICPC, Olympiad) or advanced research."
            }
        },
        new SectionRubricCriteria
        {
            Id = "exp.teamwork",
            Criterion = "Teamwork & Collaboration",
            Description = "Ability to work maturely and effectively in a team.",
            Weight0To1 = 0.3,
            Scoring = new ScoringScale
            {
                Zero  = "No teamwork evidence.",
                One   = "Basic mention of group coursework.",
                Two   = "Some collaboration in school projects.",
                Three = "Good teamwork in internships or hackathons.",
                Four  = "Strong collaboration: leadership roles or cross-functional teamwork.",
                Five  = "Exceptional: demonstrated leadership, mentoring, or impactful team contribution."
            }
        },
        new SectionRubricCriteria
        {
            Id = "exp.initiative",
            Criterion = "Initiative & Self-Motivation",
            Description = "Proactive contributions beyond assignments.",
            Weight0To1 = 0.2,
            Scoring = new ScoringScale
            {
                Zero  = "No initiative shown.",
                One   = "Minimal initiative (completed assigned tasks only).",
                Two   = "Some initiative in small side projects.",
                Three = "Good initiative: independent learning or side projects.",
                Four  = "Strong: initiated impactful projects or contributions.",
                Five  = "Exceptional: consistently proactive, initiating large-scale or innovative projects."
            }
        }
    }
    };

    private SectionRubric MockProjectsRubric() => new()
    {
        Weight0To1 = 0.20,
        Criteria = new List<SectionRubricCriteria>
    {
        new SectionRubricCriteria
        {
            Id = "proj.relevance",
            Criterion = "Technical Relevance to JD",
            Description = "Projects demonstrate OOP, problem solving, Unix/Linux, or teamwork.",
            Weight0To1 = 0.7,
            Scoring = new ScoringScale
            {
                Zero  = "No relevant projects.",
                One   = "Unrelated projects or unclear descriptions.",
                Two   = "Somewhat related, limited technical detail.",
                Three = "Relevant projects showing applied technical skills.",
                Four  = "Strongly relevant projects with good technical depth.",
                Five  = "Highly relevant, technically complex projects directly aligned with JD."
            }
        },
        new SectionRubricCriteria
        {
            Id = "proj.initiative",
            Criterion = "Initiative & Creativity in Projects",
            Description = "Evidence of self-driven or innovative project work.",
            Weight0To1 = 0.3,
            Scoring = new ScoringScale
            {
                Zero  = "Only coursework projects.",
                One   = "Minimal independent initiative in projects.",
                Two   = "Some self-initiated projects but small scope.",
                Three = "Good independent project(s) with practical use.",
                Four  = "Multiple self-driven, impactful projects.",
                Five  = "Exceptional: innovative, large-scale, or widely used independent projects."
            }
        }
    }
    };

    private SectionRubric MockSummaryRubric() => new()
    {
        Weight0To1 = 0.05,
        Criteria = new List<SectionRubricCriteria>
    {
        new SectionRubricCriteria
        {
            Id = "summary.clarity",
            Criterion = "Clarity & Relevance of Career Objective",
            Description = "Clear, relevant objective aligned with job role.",
            Weight0To1 = 1.0,
            Scoring = new ScoringScale
            {
                Zero  = "No career objective provided.",
                One   = "Vague or generic objective, not role-specific.",
                Two   = "Somewhat relevant but lacks clarity or focus.",
                Three = "Clear and relevant to the role, but could be more specific.",
                Four  = "Well-written, specific, and clearly aligned with the job role.",
                Five  = "Exceptional: concise, compelling, and perfectly tailored to the job and company."
            }
        }
    }
    };
}