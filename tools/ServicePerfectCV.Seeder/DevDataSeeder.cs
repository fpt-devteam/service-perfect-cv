using Bogus;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Helpers;
using System.Text.Json;
using ServicePerfectCV.Domain.ValueObjects;

namespace ServicePerfectCV.Seeder
{
    public class DevDataSeeder(ApplicationDbContext dbContext)
    {
        private List<Guid> _userIds = new List<Guid>();
        private List<Guid> _cvIds = new List<Guid>();
        private List<Guid> _jobDescriptionIds = new List<Guid>();
        private List<Guid> _jobTitleIds = new List<Guid>();
        private List<Guid> _employmentTypeIds = new List<Guid>();
        private List<Guid> _organizationIds = new List<Guid>();
        private List<Guid> _certificationIds = new List<Guid>();
        private List<Guid> _experienceIds = new List<Guid>();
        private List<Guid> _projectIds = new List<Guid>();
        private List<Guid> _degreeIds = new List<Guid>();
        private List<Guid> _educationIds = new List<Guid>();
        private List<Guid> _contactIds = new List<Guid>();
        private List<Guid> _skillIds = new List<Guid>();
        private List<Guid> _summaryIds = new List<Guid>();
        private List<Guid> _packageIds = new List<Guid>();
        private Guid _thangUserId = Guid.NewGuid();

        public async Task RunAsync(CancellationToken ct)
        {
            await dbContext.Database.MigrateAsync(ct);
            await ClearAllDataAsync(ct);

            await SeedUsersAsync(ct);
            await SeedEmploymentTypesAsync(ct);
            await SeedOrganizationsAsync(ct);
            await SeedJobsTitleAsync(ct);
            await SeedDegreesAsync(ct);
            await SeedCVsAsync(ct);
            await SeedJobDescriptionsAsync(ct);
            await SeedExperiencesAsync(ct);
            await SeedProjectsAsync(ct);
            await SeedCertificationsAsync(ct);
            await SeedEducationsAsync(ct);
            await SeedContactsAsync(ct);
            await SeedSkillsAsync(ct);
            await SeedSummariesAsync(ct);
            await SeedPackagesAsync(ct);

            await UpdateCVContentAsync(ct);
        }

        private async Task SeedUsersAsync(CancellationToken ct)
        {
            if (await dbContext.Users.AnyAsync(ct))
            {
                Console.WriteLine("Users already seeded.");
                _userIds = await dbContext.Users.AsNoTracking().Select(u => u.Id).ToListAsync(ct);
                return;
            }

            Faker<User>? userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f =>
                {
                    var id = Guid.NewGuid();
                    _userIds.Add(id);
                    return id;
                })
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PasswordHash, new PasswordHasher().HashPassword("Password123!"))
                .RuleFor(u => u.AuthMethod, AuthenticationMethod.JWT)
                .RuleFor(u => u.Status, UserStatus.Active)
                .RuleFor(u => u.Role, UserRole.User);

            const int total = 20;
            const int batchSize = 10;

            for (int i = 0; i < total / batchSize; i++)
            {
                List<User>? users = userFaker.Generate(batchSize);
                dbContext.Users.AddRange(users);
                await dbContext.SaveChangesAsync(ct);
            }

            User user = new User
            {
                Id = _thangUserId,
                Email = "thang@gmail.com",
                PasswordHash = new PasswordHasher().HashPassword("Thang2704!"),
                AuthMethod = AuthenticationMethod.JWT,
                Status = UserStatus.Active,
                Role = UserRole.User
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync(ct);
            _userIds.Add(_thangUserId);

            Console.WriteLine("Seeded thang User");
        }

        private async Task SeedJobsTitleAsync(CancellationToken ct)
        {
            if (await dbContext.JobTitles.AnyAsync(ct))
            {
                Console.WriteLine("Job Titles already seeded.");
                return;
            }

            string path = "./MockData/jobtitles.json";
            if (!File.Exists(path))
            {
                Console.WriteLine("Job titles seed file not found: " + path);
                return;
            }

            var json = await File.ReadAllTextAsync(path, ct);
            var jobTitleNames = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();

            var jobTitleEntities = new List<JobTitle>();
            jobTitleNames.ForEach(name =>
            {
                var id = Guid.NewGuid();
                _jobTitleIds.Add(id);
                jobTitleEntities.Add(new JobTitle
                {
                    Id = id,
                    Name = name
                });
            });

            dbContext.JobTitles.AddRange(jobTitleEntities);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded Job Titles: {jobTitleEntities.Count}");
            Console.WriteLine($"Job Title IDs generated: {string.Join(", ", _jobTitleIds)}");
        }

        private async Task SeedCVsAsync(CancellationToken ct)
        {
            if (await dbContext.CVs.AnyAsync(ct))
            {
                Console.WriteLine("CVs already seeded.");
                // Load existing CV IDs into the list
                _cvIds = await dbContext.CVs.AsNoTracking().Select(c => c.Id).ToListAsync(ct);
                return;
            }

            var cvFaker = new Faker<CV>()
                .RuleFor(cv => cv.Id, f =>
                {
                    var id = Guid.NewGuid();
                    _cvIds.Add(id);
                    return id;
                })
                .RuleFor(cv => cv.UserId, f => _thangUserId)
                .RuleFor(cv => cv.Title, f => f.Name.JobTitle())
                .RuleFor(cv => cv.CreatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(cv => cv.UpdatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(cv => cv.DeletedAt, f => null)
                .RuleFor(cv => cv.Content, f => null);

            var cvs = cvFaker.Generate(20);

            dbContext.CVs.AddRange(cvs);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded CVs: {cvs.Count}");
            Console.WriteLine($"CV IDs generated: {string.Join(", ", _cvIds)}");
        }

        private async Task SeedJobDescriptionsAsync(CancellationToken ct)
        {
            if (await dbContext.JobDescriptions.AnyAsync(ct))
            {
                Console.WriteLine("JobDescriptions already seeded.");
                _jobDescriptionIds = await dbContext.JobDescriptions.AsNoTracking().Select(jd => jd.Id).ToListAsync(ct);
                return;
            }

            if (!_cvIds.Any())
            {
                Console.WriteLine("Cannot seed job descriptions: CVs not seeded yet.");
                return;
            }

            var jobDescriptions = new List<JobDescription>();
            var faker = new Faker();

            // Create exactly one JobDescription per CV
            foreach (var cvId in _cvIds)
            {
                var jobDescriptionId = Guid.NewGuid();
                _jobDescriptionIds.Add(jobDescriptionId);

                var jobDescription = new JobDescription
                {
                    Id = jobDescriptionId,
                    CVId = cvId,
                    Title = faker.PickRandom(new[]
                    {
                        "Senior Software Developer",
                        "Full Stack Developer",
                        "Frontend Developer",
                        "Backend Developer",
                        "DevOps Engineer",
                        "Software Engineer",
                        "Senior Software Engineer",
                        "Lead Developer",
                        "Technical Lead",
                        "Software Architect",
                        "Data Engineer",
                        "Machine Learning Engineer",
                        "Mobile App Developer",
                        "Cloud Engineer",
                        "QA Engineer"
                    }),
                    CompanyName = faker.Company.CompanyName(),
                    Responsibility = faker.PickRandom(new[]
                    {
                        "Develop and maintain web applications using modern technologies. Collaborate with cross-functional teams to deliver high-quality software solutions.",
                        "Design and implement scalable backend systems. Work closely with frontend developers to create seamless user experiences.",
                        "Build responsive and interactive user interfaces. Optimize application performance and ensure cross-browser compatibility.",
                        "Manage cloud infrastructure and deployment pipelines. Implement monitoring and logging solutions for production systems.",
                        "Lead software development projects and mentor junior developers. Drive technical decisions and architectural improvements.",
                        "Analyze data requirements and build ETL pipelines. Develop data models and ensure data quality and integrity.",
                        "Design and implement machine learning models. Work with large datasets and optimize model performance.",
                        "Develop mobile applications for iOS and Android platforms. Ensure app performance and user experience optimization."
                    }),
                    Qualification = faker.PickRandom(new[]
                    {
                        "Bachelor's degree in Computer Science or related field. 3+ years of experience in software development. Strong knowledge of programming languages and frameworks.",
                        "Master's degree in Computer Science preferred. 5+ years of experience in full-stack development. Experience with cloud platforms and microservices architecture.",
                        "Bachelor's degree in Software Engineering. 2+ years of experience in frontend development. Proficiency in JavaScript, React, and CSS frameworks.",
                        "Degree in Computer Science or equivalent experience. 4+ years of experience in backend development. Strong database design and API development skills.",
                        "Bachelor's degree in Engineering. 3+ years of DevOps experience. Knowledge of containerization, CI/CD, and cloud infrastructure management.",
                        "Computer Science degree or equivalent. 6+ years of software development experience. Leadership experience and strong communication skills.",
                        "Advanced degree in Data Science or related field. 3+ years of experience with big data technologies and data processing frameworks.",
                        "Bachelor's degree in Computer Science. 4+ years of experience in machine learning and AI. Proficiency in Python, TensorFlow, and statistical analysis."
                    })
                };

                jobDescriptions.Add(jobDescription);
            }

            dbContext.JobDescriptions.AddRange(jobDescriptions);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded JobDescriptions: {jobDescriptions.Count}");
        }

        private async Task SeedExperiencesAsync(CancellationToken ct)
        {
            if (await dbContext.Experiences.AnyAsync(ct))
            {
                Console.WriteLine("Experiences already seeded.");
                return;
            }

            if (!_cvIds.Any() || !_employmentTypeIds.Any())
            {
                Console.WriteLine("Cannot seed experiences: CVs or Employment Types not seeded yet.");
                return;
            }

            var experienceFaker = new Faker<Experience>()
                .RuleFor(e => e.Id, f =>
                {
                    var id = Guid.NewGuid();
                    _experienceIds.Add(id);
                    return id;
                })
                .RuleFor(e => e.CVId, f => f.PickRandom(_cvIds))
                .RuleFor(e => e.JobTitle, f => f.Name.JobTitle())
                .RuleFor(e => e.EmploymentTypeId, f => f.PickRandom(_employmentTypeIds))
                .RuleFor(e => e.Organization, f => f.Company.CompanyName())
                .RuleFor(e => e.Location, f => f.Address.City())
                .RuleFor(e => e.StartDate, f => DateOnly.FromDateTime(f.Date.Past(5)))
                .RuleFor(e => e.EndDate, (f, e) =>
                {
                    // Generate a random duration between 1 month and 3 years for work experience
                    var durationInDays = f.Random.Int(30, 1095); // 30 days to 3 years
                    var endDate = e.StartDate.HasValue
                        ? e.StartDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(durationInDays)
                        : f.Date.Past(1);

                    // Ensure EndDate doesn't exceed current time
                    if (endDate > DateTime.Now)
                        endDate = DateTime.Now.AddDays(-f.Random.Int(1, 30));

                    return DateOnly.FromDateTime(endDate);
                })
                .RuleFor(e => e.Description, f => f.Lorem.Paragraph())
                .RuleFor(e => e.CreatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(e => e.UpdatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(e => e.DeletedAt, f => null);

            var experiences = experienceFaker.Generate(50);

            dbContext.Experiences.AddRange(experiences);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded Experiences: {experiences.Count}");
        }

        private async Task SeedProjectsAsync(CancellationToken ct)
        {
            if (await dbContext.Projects.AnyAsync(ct))
            {
                Console.WriteLine("Projects already seeded.");
                return;
            }

            if (!_cvIds.Any())
            {
                Console.WriteLine("Cannot seed projects: CVs not seeded yet.");
                return;
            }

            var projectFaker = new Faker<Project>()
                .RuleFor(p => p.Id, f =>
                {
                    var id = Guid.NewGuid();
                    _projectIds.Add(id);
                    return id;
                })
                .RuleFor(p => p.CVId, f => f.PickRandom(_cvIds))
                .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Link, f => f.Internet.Url())
                .RuleFor(p => p.StartDate, f => DateOnly.FromDateTime(f.Date.Past(2)))
                .RuleFor(p => p.EndDate, (f, p) =>
                {
                    if (!p.StartDate.HasValue) return null;

                    // Generate a random duration between 1 week and 2 years for projects
                    var durationInDays = f.Random.Int(7, 730); // 7 days to 2 years
                    var endDate = p.StartDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(durationInDays);

                    // Ensure EndDate doesn't exceed current time
                    if (endDate > DateTime.Now)
                        endDate = DateTime.Now.AddDays(-f.Random.Int(1, 30));

                    return DateOnly.FromDateTime(endDate);
                })
                .RuleFor(p => p.CreatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(p => p.UpdatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(p => p.DeletedAt, f => null);

            var projects = projectFaker.Generate(30);

            dbContext.Projects.AddRange(projects);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded Projects: {projects.Count}");
        }

        private async Task SeedEmploymentTypesAsync(CancellationToken ct)
        {
            if (await dbContext.EmploymentTypes.AnyAsync(ct))
            {
                Console.WriteLine("Employment Types already seeded.");
                return;
            }

            string path = "./MockData/employment-type.json";
            if (!File.Exists(path))
            {
                Console.WriteLine("Employment Type seed file not found: " + path);
                return;
            }

            var json = await File.ReadAllTextAsync(path, ct);
            var employmentTypeNames = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();

            var employmentTypeEntities = new List<EmploymentType>();
            employmentTypeNames.ForEach(name =>
            {
                var id = Guid.NewGuid();
                _employmentTypeIds.Add(id);
                employmentTypeEntities.Add(new EmploymentType
                {
                    Id = id,
                    Name = name
                });
            });

            dbContext.EmploymentTypes.AddRange(employmentTypeEntities);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded Employment Types: {employmentTypeEntities.Count}");
        }

        private async Task SeedOrganizationsAsync(CancellationToken ct)
        {
            if (await dbContext.Organizations.AnyAsync(ct))
            {
                Console.WriteLine("Organizations already seeded.");
                return;
            }

            var universityJson = await File.ReadAllTextAsync("./MockData/vietnam-university.json", ct);
            var companyJson = await File.ReadAllTextAsync("./MockData/companies.json", ct);
            var highSchoolJson = await File.ReadAllTextAsync("./MockData/high-schools.json", ct);
            var organizationJson = await File.ReadAllTextAsync("./MockData/organizations.json", ct);

            var universityNames = JsonSerializer.Deserialize<List<string>>(universityJson) ?? new List<string>();
            var companyNames = JsonSerializer.Deserialize<List<string>>(companyJson) ?? new List<string>();
            var highSchoolNames = JsonSerializer.Deserialize<List<string>>(highSchoolJson) ?? new List<string>();
            var organizationNames = JsonSerializer.Deserialize<List<string>>(organizationJson) ?? new List<string>();


            Console.WriteLine($"Sample organization name: '{organizationNames.FirstOrDefault()}'");
            Console.WriteLine($"Total organizations loaded: {organizationNames.Count}");

            var orgEntities = new List<Organization>();
            universityNames.ForEach(name =>
            {
                var id = Guid.NewGuid();
                _organizationIds.Add(id);
                orgEntities.Add(new Organization
                {
                    Id = id,
                    Name = name.Trim(),
                    OrganizationType = OrganizationType.University
                });
            });
            companyNames.ForEach(name =>
            {
                var id = Guid.NewGuid();
                _organizationIds.Add(id);
                orgEntities.Add(new Organization
                {
                    Id = id,
                    Name = name.Trim(),
                    OrganizationType = OrganizationType.Company
                });
            });
            highSchoolNames.ForEach(name =>
            {
                var id = Guid.NewGuid();
                _organizationIds.Add(id);
                orgEntities.Add(new Organization
                {
                    Id = id,
                    Name = name.Trim(),
                    OrganizationType = OrganizationType.School
                });
            });
            organizationNames.ForEach(name =>
            {
                var id = Guid.NewGuid();
                _organizationIds.Add(id);
                orgEntities.Add(new Organization
                {
                    Id = id,
                    Name = name.Trim(),
                    OrganizationType = OrganizationType.Organization
                });
            });

            dbContext.Organizations.AddRange(orgEntities);

            await dbContext.SaveChangesAsync(ct);
        }

        private async Task SeedDegreesAsync(CancellationToken ct)
        {
            if (await dbContext.Degrees.AnyAsync(ct))
            {
                Console.WriteLine("Degrees already seeded.");
                return;
            }

            string path = "./MockData/degrees.json";
            if (!File.Exists(path))
            {
                Console.WriteLine("Degrees seed file not found: " + path);
                return;
            }

            var json = await File.ReadAllTextAsync(path, ct);
            var degreeData = JsonSerializer.Deserialize<List<DegreeData>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<DegreeData>();

            var degreeEntities = new List<Degree>();
            degreeData.ForEach(data =>
            {
                var id = Guid.NewGuid();
                _degreeIds.Add(id);
                degreeEntities.Add(new Degree
                {
                    Id = id,
                    Code = data.Code,
                    Name = data.Name
                });
            });

            dbContext.Degrees.AddRange(degreeEntities);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded Degrees: {degreeEntities.Count}");
        }

        private async Task SeedSkillsAsync(CancellationToken ct)
        {
            if (await dbContext.Skills.AnyAsync(ct))
            {
                Console.WriteLine("Skills already seeded.");
                return;
            }

            if (!_cvIds.Any())
            {
                Console.WriteLine("Cannot seed skills: CVs not seeded yet.");
                return;
            }

            var skills = new Faker<Skill>()
                .RuleFor(s => s.Id, f =>
                {
                    var id = Guid.NewGuid();
                    _skillIds.Add(id);
                    return id;
                })
                .RuleFor(s => s.CVId, f => f.PickRandom(_cvIds))
                .RuleFor(s => s.Category, f => f.PickRandom(new[] { "Programming Languages", "Frameworks", "Databases", "Cloud Platforms", "DevOps Tools", "Soft Skills", "Other" }))
                .RuleFor(s => s.Content, f => f.PickRandom(new[]
                {
                    "Proficient in C# and .NET Core for backend development and API design.",
                    "Experienced with Java and Spring Boot for scalable enterprise applications.",
                    "Skilled in Python for scripting, automation, and data analysis tasks.",
                    "Strong knowledge of JavaScript, TypeScript, and modern frontend frameworks such as React and Angular.",
                    "Expertise in SQL and database design, including performance optimization and query tuning.",
                    "Hands-on experience with cloud platforms like Azure and AWS for deploying and managing applications.",
                    "Familiar with CI/CD pipelines, Docker, and Kubernetes for DevOps and container orchestration.",
                    "Solid understanding of RESTful API development and integration.",
                    "Knowledgeable in software architecture patterns, including microservices and event-driven systems.",
                    "Experienced in Agile methodologies, code reviews, and collaborative development practices.",
                    "Ability to write unit tests and implement test-driven development (TDD).",
                    "Skilled in troubleshooting, debugging, and optimizing application performance.",
                    "Experience with version control systems such as Git and GitHub.",
                    "Understanding of security best practices in web and cloud applications.",
                    "Ability to mentor junior developers and contribute to technical documentation."
                }))
                .RuleFor(s => s.CreatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(s => s.UpdatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(s => s.DeletedAt, f => null);

            var skillEntities = skills.Generate(50);

            dbContext.Skills.AddRange(skillEntities);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded {skillEntities.Count} skills.");
        }

        private async Task SeedCertificationsAsync(CancellationToken ct)
        {
            if (await dbContext.Certifications.AnyAsync(ct))
            {
                Console.WriteLine("Certifications already seeded.");
                return;
            }

            if (!_cvIds.Any())
            {
                Console.WriteLine("Cannot seed certifications: CVs not seeded yet.");
                return;
            }

            // Load organizations to get names
            var organizations = await dbContext.Organizations.AsNoTracking().ToListAsync(ct);
            var organizationDict = organizations.ToDictionary(o => o.Id, o => o.Name);

            var certificationFaker = new Faker<Certification>()
                .RuleFor(c => c.Id, f =>
                {
                    var id = Guid.NewGuid();
                    _certificationIds.Add(id);
                    return id;
                })
                .RuleFor(c => c.CVId, f => f.PickRandom(_cvIds))
                .RuleFor(c => c.Name, f => f.PickRandom(new[]
                {
                    "AWS Certified Solutions Architect",
                    "Microsoft Azure Fundamentals",
                    "Google Cloud Professional",
                    "Certified Information Systems Security Professional (CISSP)",
                    "Project Management Professional (PMP)",
                    "Certified ScrumMaster (CSM)",
                    "Oracle Certified Professional",
                    "Cisco Certified Network Associate (CCNA)",
                    "CompTIA Security+",
                    "Certified Ethical Hacker (CEH)",
                    "SAP Certified Application Associate",
                    "Salesforce Certified Administrator",
                    "VMware Certified Professional",
                    "Docker Certified Associate",
                    "Kubernetes Administrator Certification"
                }))
                .RuleFor(c => c.Organization, (f, c) =>
                {
                    return f.Company.CompanyName();
                })
                .RuleFor(c => c.IssuedDate, f => DateOnly.FromDateTime(f.Date.Past(3)))
                .RuleFor(c => c.Description, f => f.Lorem.Sentence())
                .RuleFor(c => c.CreatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(c => c.UpdatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(c => c.DeletedAt, f => null);

            var certifications = certificationFaker.Generate(40);

            dbContext.Certifications.AddRange(certifications);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded Certifications: {certifications.Count}");
        }

        private async Task SeedEducationsAsync(CancellationToken ct)
        {
            if (await dbContext.Educations.AnyAsync(ct))
            {
                Console.WriteLine("Educations already seeded.");
                return;
            }

            if (!_cvIds.Any())
            {
                Console.WriteLine("Cannot seed educations: CVs not seeded yet.");
                return;
            }

            if (!_degreeIds.Any())
            {
                Console.WriteLine("Cannot seed educations: Degrees not seeded yet.");
                return;
            }

            // Load degrees and organizations to get names
            var degrees = await dbContext.Degrees.AsNoTracking().ToListAsync(ct);
            var degreeDict = degrees.ToDictionary(d => d.Id, d => d.Name);

            var organizations = await dbContext.Organizations.AsNoTracking().ToListAsync(ct);
            var organizationDict = organizations.ToDictionary(o => o.Id, o => o.Name);

            var educationFaker = new Faker<Education>()
                .RuleFor(e => e.Id, f =>
                {
                    var id = Guid.NewGuid();
                    _educationIds.Add(id);
                    return id;
                })
                .RuleFor(e => e.CVId, f => f.PickRandom(_cvIds))
                .RuleFor(e => e.Degree, (f, e) =>
                {
                    return f.PickRandom(new[]
                    {
                        "Bachelor of Science",
                        "Master of Science",
                        "Bachelor of Arts",
                        "Master of Arts",
                        "Doctor of Philosophy"
                    });
                })
                .RuleFor(e => e.Organization, (f, e) =>
                {
                    return f.PickRandom(new[]
                    {
                        "Harvard University",
                        "Stanford University",
                        "MIT",
                        "University of California, Berkeley",
                        "Oxford University",
                        "Cambridge University",
                        "Yale University",
                        "Princeton University",
                        "Columbia University",
                        "University of Chicago"
                    });
                })
                .RuleFor(e => e.FieldOfStudy, f => f.PickRandom(new[]
                {
                    "Computer Science",
                    "Software Engineering",
                    "Information Technology",
                    "Data Science",
                    "Artificial Intelligence",
                    "Cybersecurity",
                    "Business Administration",
                    "Marketing",
                    "Finance",
                    "Economics",
                    "Psychology",
                    "Mechanical Engineering",
                    "Electrical Engineering",
                    "Mathematics",
                    "Physics"
                }))
                .RuleFor(p => p.StartDate, f => DateOnly.FromDateTime(f.Date.Past(5)))
                .RuleFor(e => e.EndDate, (f, e) =>
                {
                    if (!e.StartDate.HasValue) return null;

                    // Generate a random duration between 1 month and 4 years
                    var durationInDays = f.Random.Int(30, 1460); // 30 days to 4 years
                    var endDate = e.StartDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(durationInDays);

                    // Ensure EndDate doesn't exceed current time
                    if (endDate > DateTime.Now)
                        endDate = DateTime.Now.AddDays(-f.Random.Int(1, 30));

                    return DateOnly.FromDateTime(endDate);
                })
                .RuleFor(e => e.Description, f => f.Lorem.Sentences(2))
                .RuleFor(e => e.Gpa, f => f.Random.Decimal(2.0m, 4.0m))
                .RuleFor(e => e.CreatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(e => e.UpdatedAt, f => new DateTimeOffset(DateTime.SpecifyKind(f.Date.Past(1), DateTimeKind.Utc)))
                .RuleFor(e => e.DeletedAt, f => null);

            var educations = educationFaker.Generate(50);

            dbContext.Educations.AddRange(educations);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded Educations: {educations.Count}");
        }

        private async Task SeedContactsAsync(CancellationToken ct)
        {
            if (await dbContext.Contacts.AnyAsync(ct))
            {
                Console.WriteLine("Contacts already seeded.");
                return;
            }

            if (!_cvIds.Any())
            {
                Console.WriteLine("Cannot seed contacts: CVs not seeded yet.");
                return;
            }

            var contacts = new List<Contact>();
            var faker = new Faker();

            // Create exactly one contact per CV to maintain 1:1 relationship
            foreach (var cvId in _cvIds)
            {
                var contactId = Guid.NewGuid();
                _contactIds.Add(contactId);

                var email = faker.Internet.Email();
                var linkedInUsername = faker.Internet.UserName();
                var gitHubUsername = faker.Internet.UserName();
                var website = faker.Internet.Url();
                var country = faker.Address.Country();
                var city = faker.Address.City();

                var contact = new Contact
                {
                    Id = contactId,
                    CVId = cvId,
                    PhoneNumber = faker.Phone.PhoneNumber("###-###-####"), // Max 12 characters
                    Email = email.Length > 100 ? email.Substring(0, 100) : email,
                    LinkedInUrl = faker.Random.Bool(0.7f) ?
                        $"https://linkedin.com/in/{linkedInUsername}".Length > 200 ?
                        $"https://linkedin.com/in/{linkedInUsername}".Substring(0, 200) :
                        $"https://linkedin.com/in/{linkedInUsername}" : null,
                    GitHubUrl = faker.Random.Bool(0.5f) ?
                        $"https://github.com/{gitHubUsername}".Length > 200 ?
                        $"https://github.com/{gitHubUsername}".Substring(0, 200) :
                        $"https://github.com/{gitHubUsername}" : null,
                    PersonalWebsiteUrl = faker.Random.Bool(0.3f) ?
                        website.Length > 200 ? website.Substring(0, 200) : website : null,
                    Country = country.Length > 50 ? country.Substring(0, 50) : country,
                    City = city.Length > 50 ? city.Substring(0, 50) : city
                };

                contacts.Add(contact);
            }

            await dbContext.Contacts.AddRangeAsync(contacts, ct);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded {contacts.Count} Contacts");
        }

        private async Task SeedSummariesAsync(CancellationToken ct)
        {
            if (await dbContext.Summaries.AnyAsync(ct))
            {
                Console.WriteLine("Summaries already seeded.");
                return;
            }

            if (!_cvIds.Any())
            {
                Console.WriteLine("Cannot seed summaries: CVs not seeded yet.");
                return;
            }

            var summaries = new List<Summary>();
            var faker = new Faker();

            // Create exactly one summary per CV to maintain 1:1 relationship
            foreach (var cvId in _cvIds)
            {
                var summaryId = Guid.NewGuid();
                _summaryIds.Add(summaryId);

                var summary = new Summary
                {
                    Id = summaryId,
                    CVId = cvId,
                    Content = faker.PickRandom(new[]
                    {
                        "Experienced software developer with strong background in full-stack development, passionate about creating efficient and scalable solutions.",
                        "Results-driven professional with expertise in cloud technologies and modern development practices, dedicated to continuous learning and innovation.",
                        "Detail-oriented developer with extensive experience in web applications, database design, and agile methodologies.",
                        "Creative problem-solver with proven track record in delivering high-quality software solutions and leading development teams.",
                        "Technology enthusiast with deep knowledge of multiple programming languages and frameworks, committed to best practices and code quality.",
                        "Versatile software engineer with experience in both frontend and backend development, skilled in modern development tools and methodologies.",
                        "Passionate developer with strong analytical skills and experience in building robust, maintainable applications using cutting-edge technologies.",
                        "Dedicated professional with expertise in software architecture, system design, and performance optimization for large-scale applications."
                    })
                };

                summaries.Add(summary);
            }

            dbContext.Summaries.AddRange(summaries);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded {summaries.Count} summaries.");
        }

        private async Task SeedPackagesAsync(CancellationToken ct)
        {
            if (await dbContext.Packages.AnyAsync(ct))
            {
                Console.WriteLine("Packages already seeded.");
                _packageIds = await dbContext.Packages.AsNoTracking().Select(p => p.Id).ToListAsync(ct);
                return;
            }

            List<Package> packages = new List<Package>
            {
                new Package
                {
                    Id = Guid.NewGuid(),
                    Name = "Basic",
                    Price = 19000,
                    NumCredits = 3,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                new Package
                {
                    Id = Guid.NewGuid(),
                    Name = "Starter",
                    Price = 29000,
                    NumCredits = 5,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                new Package
                {
                    Id = Guid.NewGuid(),
                    Name = "Pro",
                    Price = 49000,
                    NumCredits = 10,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                },
                new Package
                {
                    Id = Guid.NewGuid(),
                    Name = "Expert",
                    Price = 79000,
                    NumCredits = 20,
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                }
            };

            _packageIds = packages.Select(p => p.Id).ToList();

            dbContext.Packages.AddRange(packages);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded {packages.Count} packages.");
        }

        private async Task UpdateCVContentAsync(CancellationToken ct)
        {
            Console.WriteLine("Updating CV Content...");

            // Get all CVs with their related data
            var cvs = await dbContext.CVs
                .Include(c => c.Contact)
                .Include(c => c.Summary)
                .Include(c => c.JobDescription)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                    .ThenInclude(e => e.EmploymentType)
                .Include(c => c.Projects)
                .Include(c => c.Skills)
                .Include(c => c.Certifications)
                .AsNoTracking()
                .ToListAsync(ct);

            Console.WriteLine($"Found {cvs.Count} CVs to update");

            foreach (var cv in cvs)
            {
                var cvContent = new CVContent
                {
                    Contact = cv.Contact != null ? new ContactInfo
                    {
                        PhoneNumber = cv.Contact.PhoneNumber,
                        Email = cv.Contact.Email,
                        LinkedInUrl = cv.Contact.LinkedInUrl,
                        GitHubUrl = cv.Contact.GitHubUrl,
                        PersonalWebsiteUrl = cv.Contact.PersonalWebsiteUrl,
                        Country = cv.Contact.Country,
                        City = cv.Contact.City
                    } : null,
                    Summary = cv.Summary != null ? new SummaryInfo
                    {
                        Content = cv.Summary.Content
                    } : null,
                    Educations = cv.Educations.Select(e => new EducationInfo
                    {
                        Degree = e.Degree,
                        Organization = e.Organization,
                        FieldOfStudy = e.FieldOfStudy,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Description = e.Description,
                        Gpa = e.Gpa
                    }).ToList(),
                    Experiences = cv.Experiences.Select(e => new ExperienceInfo
                    {
                        JobTitle = e.JobTitle,
                        EmploymentTypeId = e.EmploymentTypeId,
                        Organization = e.Organization,
                        Location = e.Location,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Description = e.Description
                    }).ToList(),
                    Projects = cv.Projects.Select(p => new ProjectInfo
                    {
                        Title = p.Title,
                        Description = p.Description,
                        Link = p.Link,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate
                    }).ToList(),
                    Skills = cv.Skills.Select(s => new SkillInfo
                    {
                        Content = s.Content,
                        Category = s.Category
                    }).ToList(),
                    Certifications = cv.Certifications.Select(c => new CertificationInfo
                    {
                        Name = c.Name,
                        Organization = c.Organization,
                        IssuedDate = c.IssuedDate,
                        Description = c.Description
                    }).ToList()
                };

                // Update the CV's Content
                await dbContext.CVs
                    .Where(c => c.Id == cv.Id)
                    .ExecuteUpdateAsync(c => c
                        .SetProperty(cv => cv.Content, cvContent)
                        .SetProperty(cv => cv.UpdatedAt, DateTimeOffset.UtcNow), ct);
            }

            Console.WriteLine($"Updated Content for {cvs.Count} CVs");
        }

        public async Task ClearAllDataAsync(CancellationToken ct)
        {
            Console.WriteLine("Starting to clear all data from database...");

            await dbContext.Projects.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Projects");

            await dbContext.Experiences.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Experiences");

            await dbContext.Certifications.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Certifications");

            await dbContext.Educations.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Educations");

            await dbContext.Contacts.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Contacts");

            await dbContext.Skills.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Skills");

            await dbContext.Summaries.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Summaries");

            await dbContext.Packages.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Packages");

            await dbContext.JobDescriptions.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared JobDescriptions");

            await dbContext.CVs.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared CVs");

            await dbContext.JobTitles.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared JobTitles");

            await dbContext.Organizations.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Organizations");

            await dbContext.EmploymentTypes.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared EmploymentTypes");

            await dbContext.Degrees.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Degrees");

            await dbContext.Users.ExecuteDeleteAsync(ct);
            Console.WriteLine("Cleared Users");

            _userIds.Clear();
            _cvIds.Clear();
            _jobDescriptionIds.Clear();
            _jobTitleIds.Clear();
            _employmentTypeIds.Clear();
            _organizationIds.Clear();
            _experienceIds.Clear();
            _projectIds.Clear();
            _degreeIds.Clear();
            _certificationIds.Clear();
            _educationIds.Clear();
            _contactIds.Clear();
            _skillIds.Clear();
            _summaryIds.Clear();
            _packageIds.Clear();

            Console.WriteLine("All data cleared successfully!");
        }
    }
}