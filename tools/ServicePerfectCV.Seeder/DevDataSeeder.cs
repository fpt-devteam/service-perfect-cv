using Bogus;
using Microsoft.EntityFrameworkCore;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Infrastructure.Data;
using ServicePerfectCV.Infrastructure.Helpers;
using System.Text.Json;

namespace ServicePerfectCV.Seeder
{
    public class DevDataSeeder(ApplicationDbContext dbContext)
    {
        public async Task RunAsync(CancellationToken ct)
        {
            await dbContext.Database.MigrateAsync(ct);
            await SeedUsersAsync(ct);
            await SeedEmploymentTypesAsync(ct);
            await SeedOrganizationsAsync(ct);
            await SeedCVsAsync(ct);
            await SeedExperiencesAsync(ct);
            await SeedProjectsAsync(ct);
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
        private async Task SeedCVsAsync(CancellationToken ct)
        {
            if (await dbContext.CVs.AnyAsync(ct))
            {
                Console.WriteLine("CVs already seeded.");
                return;
            }

            string path = "cvs.json";
            if (!File.Exists(path))
            {
                Console.WriteLine("CV seed file not found: " + path);
                return;
            }

            string json = await File.ReadAllTextAsync(path, ct);

            List<CV> cvs = JsonSerializer.Deserialize<List<CV>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<CV>();

            dbContext.CVs.AddRange(cvs);
            await dbContext.SaveChangesAsync(ct);

            Console.WriteLine("Seeded CVs: " + cvs.Count);
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