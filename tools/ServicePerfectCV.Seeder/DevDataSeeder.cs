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
        private List<Guid> _cvIds = new List<Guid>(); // Lưu trữ CV IDs để sử dụng cho seeding khác
        private List<Guid> _jobTitleIds = new List<Guid>(); // Lưu trữ Job Title IDs

        public async Task RunAsync(CancellationToken ct)
        {
            await dbContext.Database.MigrateAsync(ct);
            // await SeedUsersAsync(ct);
            // await SeedEmploymentTypesAsync(ct);
            // await SeedOrganizationsAsync(ct);
            // await SeedJobsTitleAsync(ct);
            await SeedCVsAsync(ct);
            // await SeedExperiencesAsync(ct);
            // await SeedProjectsAsync(ct);
            // await SeedDegreeAsync(ct);
        }

        private async Task SeedUsersAsync(CancellationToken ct)
        {
            if (await dbContext.Users.AnyAsync(ct))
            {
                Console.WriteLine("Users already seeded.");
                return;
            }

            Faker<User>? userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => Guid.NewGuid())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PasswordHash, new PasswordHasher().HashPassword("123456"))
                .RuleFor(u => u.Status, UserStatus.Active)
                .RuleFor(u => u.Role, UserRole.User);

            const int total = 20;
            const int batchSize = 10;

            for (int i = 0; i < total / batchSize; i++)
            {
                List<User>? users = userFaker.Generate(batchSize);
                dbContext.Users.AddRange(users);
                await dbContext.SaveChangesAsync(ct);

                Console.WriteLine($"Inserted {(i + 1) * batchSize}/{total}");
            }
        }
        private async Task SeedJobsTitleAsync(CancellationToken ct)
        {
            if (await dbContext.JobTitles.AnyAsync(ct))
            {
                Console.WriteLine("Job Titles already seeded.");
                return;
            }

            string path = "jobtitles.json";
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
            // if (await dbContext.CVs.AnyAsync(ct))
            // {
            //     Console.WriteLine("CVs already seeded.");
            //     return;
            // }

            var userId = new Guid("AD63B027-62DA-4AF5-8D63-FC9D3C23403A");

            var cvFaker = new Faker<CV>()
                .RuleFor(cv => cv.Id, f =>
                {
                    var id = Guid.NewGuid();
                    _cvIds.Add(id);
                    return id;
                })
                .RuleFor(cv => cv.UserId, f => userId)
                .RuleFor(cv => cv.Title, f => f.Name.JobTitle())
                .RuleFor(cv => cv.JobDetail, f => new JobDetail(
                    f.Name.JobTitle(),
                    f.Company.CompanyName(),
                    f.Name.JobDescriptor()
                ))
                .RuleFor(cv => cv.CreatedAt, f => f.Date.Past(1))
                .RuleFor(cv => cv.UpdatedAt, f => f.Date.Past(1))
                .RuleFor(cv => cv.DeletedAt, f => null)
                .RuleFor(cv => cv.FullContent, f => null);

            var cvs = cvFaker.Generate(20);

            dbContext.CVs.AddRange(cvs);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine($"Seeded CVs: {cvs.Count}");
            Console.WriteLine($"CV IDs generated: {string.Join(", ", _cvIds)}");
        }

        private async Task SeedExperiencesAsync(CancellationToken ct)
        {
            if (await dbContext.Experiences.AnyAsync(ct))
            {
                Console.WriteLine("Experiences already seeded.");
                return;
            }

            string path = "experiences.json";
            if (!File.Exists(path))
            {
                Console.WriteLine("Experience seed file not found: " + path);
                return;
            }

            string json = await File.ReadAllTextAsync(path, ct);

            List<Experience> experiences = JsonSerializer.Deserialize<List<Experience>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Experience>();

            dbContext.Experiences.AddRange(experiences);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine("Seeded Experiences: " + experiences.Count);
        }

        private async Task SeedProjectsAsync(CancellationToken ct)
        {
            if (await dbContext.Projects.AnyAsync(ct))
            {
                Console.WriteLine("Projects already seeded.");
                return;
            }

            string path = "projects.json";
            if (!File.Exists(path))
            {
                Console.WriteLine("Project seed file not found: " + path);
                return;
            }

            string json = await File.ReadAllTextAsync(path, ct);

            List<Project> projects = JsonSerializer.Deserialize<List<Project>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Project>();

            dbContext.Projects.AddRange(projects);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine("Seeded projects: " + projects.Count);
        }


        private async Task SeedEmploymentTypesAsync(CancellationToken ct)
        {
            if (await dbContext.EmploymentTypes.AnyAsync(ct))
            {
                Console.WriteLine("Employment Types already seeded.");
                return;
            }

            string path = "employment-type.json";
            if (!File.Exists(path))
            {
                Console.WriteLine("Employment Type seed file not found: " + path);
                return;
            }

            var json = await File.ReadAllTextAsync(path, ct);
            var employmentTypeNames = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();

            var employmentTypeEntities = new List<EmploymentType>();
            employmentTypeNames.ForEach(name => employmentTypeEntities.Add(new EmploymentType
            {
                Id = Guid.NewGuid(),
                Name = name
            }));

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

            var universityJson = await File.ReadAllTextAsync("vietnam-university.json", ct);
            var companyJson = await File.ReadAllTextAsync("companies.json", ct);
            var highSchoolJson = await File.ReadAllTextAsync("high-schools.json", ct);
            var organizationJson = await File.ReadAllTextAsync("organizations.json", ct);

            var universityNames = JsonSerializer.Deserialize<List<string>>(universityJson) ?? new List<string>();
            var companyNames = JsonSerializer.Deserialize<List<string>>(companyJson) ?? new List<string>();
            var highSchoolNames = JsonSerializer.Deserialize<List<string>>(highSchoolJson) ?? new List<string>();
            var organizationNames = JsonSerializer.Deserialize<List<string>>(organizationJson) ?? new List<string>();

            // Debug logging
            Console.WriteLine($"Sample organization name: '{organizationNames.FirstOrDefault()}'");
            Console.WriteLine($"Total organizations loaded: {organizationNames.Count}");

            var orgEntities = new List<Organization>();
            universityNames.ForEach(name => orgEntities.Add(new Organization
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                OrganizationType = OrganizationType.University
            }));
            companyNames.ForEach(name => orgEntities.Add(new Organization
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                OrganizationType = OrganizationType.Company
            }));
            highSchoolNames.ForEach(name => orgEntities.Add(new Organization
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                OrganizationType = OrganizationType.School
            }));
            organizationNames.ForEach(name => orgEntities.Add(new Organization
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(), // Trim whitespace
                OrganizationType = OrganizationType.Organization
            }));

            dbContext.Organizations.AddRange(orgEntities);

            await dbContext.SaveChangesAsync(ct);
        }
    }
}