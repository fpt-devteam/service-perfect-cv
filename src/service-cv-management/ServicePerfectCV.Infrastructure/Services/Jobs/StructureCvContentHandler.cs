using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.DTOs.CvStructuring;
using ServicePerfectCV.Application.DTOs.Job;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using System.Text.Json;

namespace ServicePerfectCV.Infrastructure.Services.Jobs
{
    public sealed class StructureCvContentHandler : IJobHandler
    {
        private readonly ICvStructuringService _cvStructuringService;
        private readonly IContactRepository _contactRepository;
        private readonly ISummaryRepository _summaryRepository;
        private readonly IEducationRepository _educationRepository;
        private readonly IExperienceRepository _experienceRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ICertificationRepository _certificationRepository;
        private readonly IEmploymentTypeRepository _employmentTypeRepository;
        private readonly IDegreeRepository _degreeRepository;
        private readonly ICVRepository _cvRepository;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<StructureCvContentHandler> _logger;

        public JobType JobType => JobType.StructureCvContent;

        public StructureCvContentHandler(
            ICvStructuringService cvStructuringService,
            IContactRepository contactRepository,
            ISummaryRepository summaryRepository,
            IEducationRepository educationRepository,
            IExperienceRepository experienceRepository,
            ISkillRepository skillRepository,
            IProjectRepository projectRepository,
            ICertificationRepository certificationRepository,
            IEmploymentTypeRepository employmentTypeRepository,
            IDegreeRepository degreeRepository,
            ICVRepository cvRepository,
            IJsonHelper jsonHelper,
            ILogger<StructureCvContentHandler> logger)
        {
            _cvStructuringService = cvStructuringService;
            _contactRepository = contactRepository;
            _summaryRepository = summaryRepository;
            _educationRepository = educationRepository;
            _experienceRepository = experienceRepository;
            _skillRepository = skillRepository;
            _projectRepository = projectRepository;
            _certificationRepository = certificationRepository;
            _employmentTypeRepository = employmentTypeRepository;
            _degreeRepository = degreeRepository;
            _cvRepository = cvRepository;
            _jsonHelper = jsonHelper;
            _logger = logger;
        }

        public async Task<JobHandlerResult> HandleAsync(Job job, CancellationToken cancellationToken)
        {
            var resultSummary = new Dictionary<string, object>
            {
                ["sectionsCreated"] = new List<string>(),
                ["sectionsSkipped"] = new List<string>(),
                ["errors"] = new List<string>()
            };

            try
            {
                StructureCvContentInputDto? input = _jsonHelper.DeserializeFromElement<StructureCvContentInputDto>(job.Input.RootElement);

                if (input == null || string.IsNullOrWhiteSpace(input.RawText))
                {
                    return JobHandlerResult.Failure("structure_cv.invalid_input", "Invalid job input or empty raw text",
                        _jsonHelper.SerializeToDocument(resultSummary));
                }

                _logger.LogInformation("Starting CV content structuring for CV {CvId}", input.CvId);

                // Call LLM to extract sections
                var structuringResult = await _cvStructuringService.StructureCvContentAsync(input.RawText, cancellationToken);

                if (!structuringResult.IsSuccess)
                {
                    ((List<string>)resultSummary["errors"]).AddRange(structuringResult.Errors);
                    return JobHandlerResult.Failure("structure_cv.structuring_failed", "CV structuring failed",
                        _jsonHelper.SerializeToDocument(resultSummary));
                }

                _logger.LogInformation("CV structuring completed. Extracted {Count} sections", structuringResult.Sections.Count);

                // Process each section and store in database
                foreach (var section in structuringResult.Sections)
                {
                    try
                    {
                        await ProcessSectionAsync(section.Key, section.Value, input.CvId, input.UserId);
                        ((List<string>)resultSummary["sectionsCreated"]).Add(section.Key.ToString());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing section {SectionType}", section.Key);
                        ((List<string>)resultSummary["sectionsSkipped"]).Add(section.Key.ToString());
                        ((List<string>)resultSummary["errors"]).Add($"{section.Key}: {ex.Message}");
                    }
                }

                // Mark CV as structured
                var cv = await _cvRepository.GetByIdAsync(input.CvId);
                if (cv != null)
                {
                    cv.IsStructuredDone = true;
                    cv.UpdatedAt = DateTimeOffset.UtcNow;
                    _cvRepository.Update(cv);
                    await _cvRepository.SaveChangesAsync();
                    _logger.LogInformation("CV {CvId} marked as structured", input.CvId);
                }

                return JobHandlerResult.Success(_jsonHelper.SerializeToDocument(resultSummary));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CV content structuring handler");
                ((List<string>)resultSummary["errors"]).Add(ex.Message);
                return JobHandlerResult.Failure("structure_cv.error", ex.Message,
                    _jsonHelper.SerializeToDocument(resultSummary));
            }
        }

        private async Task ProcessSectionAsync(SectionType sectionType, JsonDocument jsonDoc, Guid cvId, Guid userId)
        {
            switch (sectionType)
            {
                case SectionType.Contact:
                    await ProcessContactAsync(jsonDoc, cvId);
                    break;
                case SectionType.Summary:
                    await ProcessSummaryAsync(jsonDoc, cvId);
                    break;
                case SectionType.Education:
                    await ProcessEducationAsync(jsonDoc, cvId, userId);
                    break;
                case SectionType.Experience:
                    await ProcessExperienceAsync(jsonDoc, cvId, userId);
                    break;
                case SectionType.Skills:
                    await ProcessSkillsAsync(jsonDoc, cvId, userId);
                    break;
                case SectionType.Projects:
                    await ProcessProjectsAsync(jsonDoc, cvId, userId);
                    break;
                case SectionType.Certifications:
                    await ProcessCertificationsAsync(jsonDoc, cvId, userId);
                    break;
                default:
                    _logger.LogWarning("Unknown section type: {SectionType}", sectionType);
                    break;
            }
        }

        private async Task ProcessContactAsync(JsonDocument jsonDoc, Guid cvId)
        {
            var dto = _jsonHelper.Deserialize<ExtractedContactDto>(jsonDoc.RootElement.GetRawText());
            if (dto == null) return;

            var existing = await _contactRepository.GetByCVIdAsync(cvId);
            if (existing != null)
            {
                existing.PhoneNumber = dto.PhoneNumber;
                existing.Email = dto.Email;
                existing.LinkedInUrl = dto.LinkedInUrl;
                existing.GitHubUrl = dto.GitHubUrl;
                existing.PersonalWebsiteUrl = dto.PersonalWebsiteUrl;
                existing.Country = dto.Country;
                existing.City = dto.City;
                existing.UpdatedAt = DateTimeOffset.UtcNow;
                _contactRepository.Update(existing);
            }
            else
            {
                var contact = new Contact
                {
                    Id = Guid.NewGuid(),
                    CVId = cvId,
                    PhoneNumber = dto.PhoneNumber,
                    Email = dto.Email,
                    LinkedInUrl = dto.LinkedInUrl,
                    GitHubUrl = dto.GitHubUrl,
                    PersonalWebsiteUrl = dto.PersonalWebsiteUrl,
                    Country = dto.Country,
                    City = dto.City,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };
                await _contactRepository.CreateAsync(contact);
            }

            await _contactRepository.SaveChangesAsync();
        }

        private async Task ProcessSummaryAsync(JsonDocument jsonDoc, Guid cvId)
        {
            var dto = _jsonHelper.Deserialize<ExtractedSummaryDto>(jsonDoc.RootElement.GetRawText());
            if (dto == null || string.IsNullOrWhiteSpace(dto.Content)) return;

            var existing = await _summaryRepository.GetByCVIdAsync(cvId);
            if (existing != null)
            {
                existing.Content = dto.Content;
                existing.UpdatedAt = DateTimeOffset.UtcNow;
                _summaryRepository.Update(existing);
            }
            else
            {
                var summary = new Summary
                {
                    Id = Guid.NewGuid(),
                    CVId = cvId,
                    Content = dto.Content,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };
                await _summaryRepository.CreateAsync(summary);
            }

            await _summaryRepository.SaveChangesAsync();
        }

        private async Task ProcessEducationAsync(JsonDocument jsonDoc, Guid cvId, Guid userId)
        {
            var dtoList = _jsonHelper.Deserialize<List<ExtractedEducationDto>>(jsonDoc.RootElement.GetRawText());
            if (dtoList == null || dtoList.Count == 0) return;

            foreach (var dto in dtoList)
            {
                // Convert GPA string to decimal
                decimal? gpa = null;
                if (!string.IsNullOrWhiteSpace(dto.Gpa) && decimal.TryParse(dto.Gpa, out var parsedGpa))
                {
                    gpa = parsedGpa;
                }

                var education = new Education
                {
                    Id = Guid.NewGuid(),
                    CVId = cvId,
                    Organization = dto.Organization ?? string.Empty,
                    Degree = dto.Degree ?? string.Empty,
                    FieldOfStudy = dto.FieldOfStudy,
                    StartDate = dto.StartDate.HasValue ? DateOnly.FromDateTime(dto.StartDate.Value) : null,
                    EndDate = dto.EndDate.HasValue ? DateOnly.FromDateTime(dto.EndDate.Value) : null,
                    Description = dto.Description,
                    Gpa = gpa,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                await _educationRepository.CreateAsync(education);
            }

            await _educationRepository.SaveChangesAsync();
        }

        private async Task ProcessExperienceAsync(JsonDocument jsonDoc, Guid cvId, Guid userId)
        {
            var dtoList = _jsonHelper.Deserialize<List<ExtractedExperienceDto>>(jsonDoc.RootElement.GetRawText());
            if (dtoList == null || dtoList.Count == 0) return;

            foreach (var dto in dtoList)
            {
                // Find employment type using SearchByNameAsync
                Guid? employmentTypeId = null;
                if (!string.IsNullOrWhiteSpace(dto.EmploymentType))
                {
                    var query = new Application.DTOs.EmploymentType.Requests.EmploymentTypeQuery
                    {
                        SearchTerm = dto.EmploymentType
                    };
                    var employmentTypes = await _employmentTypeRepository.SearchByNameAsync(query);
                    var employmentType = employmentTypes.FirstOrDefault();

                    if (employmentType != null)
                    {
                        employmentTypeId = employmentType.Id;
                    }
                }

                // Skip if no valid employment type found (since it's required)
                if (!employmentTypeId.HasValue)
                {
                    _logger.LogWarning("Employment type '{Type}' not found, skipping experience", dto.EmploymentType);
                    continue;
                }

                var experience = new Experience
                {
                    Id = Guid.NewGuid(),
                    CVId = cvId,
                    JobTitle = dto.JobTitle ?? string.Empty,
                    EmploymentTypeId = employmentTypeId.Value,
                    Organization = dto.Organization ?? string.Empty,
                    Location = dto.Location,
                    StartDate = dto.StartDate.HasValue ? DateOnly.FromDateTime(dto.StartDate.Value) : null,
                    EndDate = dto.EndDate.HasValue ? DateOnly.FromDateTime(dto.EndDate.Value) : null,
                    Description = dto.Description,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                await _experienceRepository.CreateAsync(experience);
            }

            await _experienceRepository.SaveChangesAsync();
        }

        private async Task ProcessSkillsAsync(JsonDocument jsonDoc, Guid cvId, Guid userId)
        {
            var dtoList = _jsonHelper.Deserialize<List<ExtractedSkillDto>>(jsonDoc.RootElement.GetRawText());
            if (dtoList == null || dtoList.Count == 0) return;

            foreach (var dto in dtoList)
            {
                if (string.IsNullOrWhiteSpace(dto.Content)) continue;

                var skill = new Skill
                {
                    Id = Guid.NewGuid(),
                    CVId = cvId,
                    Category = dto.Category ?? "General",
                    Content = dto.Content,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                await _skillRepository.CreateAsync(skill);
            }

            await _skillRepository.SaveChangesAsync();
        }

        private async Task ProcessProjectsAsync(JsonDocument jsonDoc, Guid cvId, Guid userId)
        {
            var dtoList = _jsonHelper.Deserialize<List<ExtractedProjectDto>>(jsonDoc.RootElement.GetRawText());
            if (dtoList == null || dtoList.Count == 0) return;

            foreach (var dto in dtoList)
            {
                if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description)) continue;

                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    CVId = cvId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Link = dto.Link,
                    StartDate = dto.StartDate.HasValue ? DateOnly.FromDateTime(dto.StartDate.Value) : null,
                    EndDate = dto.EndDate.HasValue ? DateOnly.FromDateTime(dto.EndDate.Value) : null,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                await _projectRepository.CreateAsync(project);
            }

            await _projectRepository.SaveChangesAsync();
        }

        private async Task ProcessCertificationsAsync(JsonDocument jsonDoc, Guid cvId, Guid userId)
        {
            var dtoList = _jsonHelper.Deserialize<List<ExtractedCertificationDto>>(jsonDoc.RootElement.GetRawText());
            if (dtoList == null || dtoList.Count == 0) return;

            foreach (var dto in dtoList)
            {
                if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Organization)) continue;

                var certification = new Certification
                {
                    Id = Guid.NewGuid(),
                    CVId = cvId,
                    Name = dto.Name,
                    Organization = dto.Organization,
                    IssuedDate = dto.IssuedDate.HasValue ? DateOnly.FromDateTime(dto.IssuedDate.Value) : null,
                    Description = dto.Description,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };

                await _certificationRepository.CreateAsync(certification);
            }

            await _certificationRepository.SaveChangesAsync();
        }
    }
}
