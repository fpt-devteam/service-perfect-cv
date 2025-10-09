using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.DTOs.Certification.Requests;
using ServicePerfectCV.Application.DTOs.Certification.Responses;
using ServicePerfectCV.Application.DTOs.Contact.Responses;
using ServicePerfectCV.Application.DTOs.Education.Requests;
using ServicePerfectCV.Application.DTOs.Education.Responses;
using ServicePerfectCV.Application.DTOs.Experience.Responses;
using ServicePerfectCV.Application.DTOs.Job;
using ServicePerfectCV.Application.DTOs.Project.Requests;
using ServicePerfectCV.Application.DTOs.Project.Responses;
using ServicePerfectCV.Application.DTOs.Section;
using ServicePerfectCV.Application.DTOs.Skill.Requests;
using ServicePerfectCV.Application.DTOs.Skill.Responses;
using ServicePerfectCV.Application.DTOs.Summary.Responses;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Application.Interfaces.AI;
using ServicePerfectCV.Application.Interfaces.Jobs;
using ServicePerfectCV.Application.Interfaces.Repositories;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;
using ServicePerfectCV.Infrastructure.Services.AI;
using System.Text.Json;

namespace ServicePerfectCV.Infrastructure.Services.Jobs
{
    public sealed class ScoreCvSectionHandler : IJobHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<ScoreCvSectionHandler> _logger;

        public JobType JobType => JobType.ScoreCV;

        public ScoreCvSectionHandler(
            IServiceScopeFactory serviceScopeFactory,
            IJsonHelper jsonHelper,
            ILogger<ScoreCvSectionHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _jsonHelper = jsonHelper;
            _logger = logger;
        }

        public async Task<JobHandlerResult> HandleAsync(Job job, CancellationToken cancellationToken)
        {
            var allScoreResults = new Dictionary<SectionType, SectionScore>();

            try
            {
                ScoreSectionInputDto? scoreSectionInputDto = _jsonHelper.DeserializeFromElement<ScoreSectionInputDto>(job.Input.RootElement);

                if (scoreSectionInputDto == null)
                {
                    return JobHandlerResult.Failure("score_cv.invalid_input", "Invalid job input",
                        _jsonHelper.SerializeToDocument(allScoreResults));
                }

                JobDescription? jobDescription;
                Dictionary<SectionType, SectionScoreResult> existingResultsDict;
                string jdHash;

                // Step 1 & 2: Load job description and existing results using main scope
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var jobDescriptionRepository = scope.ServiceProvider.GetRequiredService<IJobDescriptionRepository>();
                    var sectionScoreResultRepository = scope.ServiceProvider.GetRequiredService<ISectionScoreResultRepository>();
                    var objectHasher = scope.ServiceProvider.GetRequiredService<IObjectHasher>();

                    jobDescription = await jobDescriptionRepository.GetByCVIdAsync(scoreSectionInputDto.CvId);
                    if (jobDescription?.SectionRubrics == null)
                    {
                        return JobHandlerResult.Failure("score_cv.no_rubric", "Please create a job description with meaningful rubrics before scoring.",
                            _jsonHelper.SerializeToDocument(allScoreResults));
                    }

                    List<SectionScoreResult> existingResults = await sectionScoreResultRepository.GetByCVIdAsync(scoreSectionInputDto.CvId);
                    existingResultsDict = existingResults.ToDictionary(r => r.SectionType, r => r);
                    jdHash = objectHasher.Hash(jobDescription);
                }

                // Step 3: Load all section content concurrently (each with its own scope)
                var sectionTypes = Enum.GetValues<SectionType>()
                    .Where(st => jobDescription.SectionRubrics.ContainsKey(st))
                    .ToList();

                _logger.LogInformation("Loading {Count} sections concurrently for CV {CvId}", sectionTypes.Count, scoreSectionInputDto.CvId);

                var sectionContentTasks = sectionTypes
                    .Select(async sectionType =>
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var content = await GetSectionContentAsync(sectionType, scoreSectionInputDto.CvId, scoreSectionInputDto.UserId, scope.ServiceProvider);
                        return new
                        {
                            SectionType = sectionType,
                            Content = content
                        };
                    })
                    .ToList();

                var sectionContents = await Task.WhenAll(sectionContentTasks);

                // Step 4: Score all sections concurrently (each with its own scope)
                var scoringTasks = sectionContents
                    .Where(sc => sc.Content != null)
                    .Select(async sc => await ScoreSectionAsync(
                        sc.SectionType,
                        sc.Content!,
                        jobDescription.SectionRubrics[sc.SectionType],
                        jdHash,
                        scoreSectionInputDto.CvId,
                        existingResultsDict,
                        cancellationToken))
                    .ToList();

                _logger.LogInformation("Scoring {Count} sections concurrently for CV {CvId}", scoringTasks.Count, scoreSectionInputDto.CvId);

                var scoringResults = await Task.WhenAll(scoringTasks);

                // Step 5: Collect all results
                foreach (var result in scoringResults)
                {
                    if (result.Success)
                    {
                        allScoreResults[result.SectionType] = result.Score!;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to score section {SectionType}: {Error}", result.SectionType, result.Error);
                        allScoreResults[result.SectionType] = new SectionScore();
                    }
                }

                _logger.LogInformation("Completed scoring {Count} sections for CV {CvId}", allScoreResults.Count, scoreSectionInputDto.CvId);

                return JobHandlerResult.Success(_jsonHelper.SerializeToDocument(allScoreResults));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (JsonException ex)
            {
                return JobHandlerResult.Failure("score_cv.json_error", $"JSON parsing error: {ex.Message}",
                    _jsonHelper.SerializeToDocument(allScoreResults));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CV scoring for job {JobId}", job.Id);
                return JobHandlerResult.Failure("score_cv.error", ex.Message,
                    _jsonHelper.SerializeToDocument(allScoreResults));
            }
        }

        private async Task<SectionScoringResult> ScoreSectionAsync(
            SectionType sectionType,
            object sectionContent,
            SectionRubric sectionRubric,
            string jdHash,
            Guid cvId,
            Dictionary<SectionType, SectionScoreResult> existingResultsDict,
            CancellationToken cancellationToken)
        {
            try
            {
                // Create a new scope for this section's scoring operation
                using var scope = _serviceScopeFactory.CreateScope();
                var sectionScoreService = scope.ServiceProvider.GetRequiredService<ISectionScoreService>();
                var sectionScoreResultRepository = scope.ServiceProvider.GetRequiredService<ISectionScoreResultRepository>();
                var objectHasher = scope.ServiceProvider.GetRequiredService<IObjectHasher>();

                string sectionContentHash = objectHasher.Hash(sectionContent);

                bool needsUpdate = true;
                SectionScoreResult? existingResult = null;

                if (existingResultsDict.TryGetValue(sectionType, out existingResult))
                {
                    needsUpdate = existingResult.JdHash != jdHash ||
                                existingResult.SectionContentHash != sectionContentHash;
                }

                SectionScore scoreResult = existingResult?.SectionScore ?? new();

                if (needsUpdate)
                {
                    var sectionContentString = _jsonHelper.Serialize(sectionContent);
                    var sectionRubricString = _jsonHelper.Serialize(sectionRubric);

                    scoreResult = await sectionScoreService.ScoreSectionAsync(
                        sectionRubricString, sectionContentString, sectionType.ToString(), cancellationToken);

                    scoreResult.Weight0To1 = sectionRubric.Weight0To1;

                    var sectionScoreResultEntity = new SectionScoreResult
                    {
                        Id = existingResult?.Id ?? Guid.NewGuid(),
                        CVId = cvId,
                        SectionType = sectionType,
                        JdHash = jdHash,
                        SectionContentHash = sectionContentHash,
                        SectionScore = scoreResult,
                        CreatedAt = existingResult?.CreatedAt ?? DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow
                    };

                    if (existingResult != null)
                    {
                        sectionScoreResultRepository.Update(sectionScoreResultEntity);
                    }
                    else
                    {
                        await sectionScoreResultRepository.CreateAsync(sectionScoreResultEntity);
                    }

                    // Save changes for this section
                    await sectionScoreResultRepository.SaveChangesAsync();
                }

                return new SectionScoringResult
                {
                    Success = true,
                    SectionType = sectionType,
                    Score = scoreResult
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scoring section {SectionType}", sectionType);
                return new SectionScoringResult
                {
                    Success = false,
                    SectionType = sectionType,
                    Error = ex.Message
                };
            }
        }

        private class SectionScoringResult
        {
            public bool Success { get; set; }
            public SectionType SectionType { get; set; }
            public SectionScore? Score { get; set; }
            public string? Error { get; set; }
        }

        private async Task<object?> GetSectionContentAsync(SectionType sectionType, Guid cvId, Guid userId, IServiceProvider serviceProvider)
        {
            return sectionType switch
            {
                SectionType.Contact => await GetContactSectionAsync(cvId, userId, serviceProvider),
                SectionType.Summary => await GetSummarySectionAsync(cvId, userId, serviceProvider),
                SectionType.Education => await GetEducationSectionAsync(cvId, userId, serviceProvider),
                SectionType.Experience => await GetExperienceSectionAsync(cvId, userId, serviceProvider),
                SectionType.Skills => await GetSkillsSectionAsync(cvId, userId, serviceProvider),
                SectionType.Projects => await GetProjectsSectionAsync(cvId, userId, serviceProvider),
                SectionType.Certifications => await GetCertificationsSectionAsync(cvId, userId, serviceProvider),
                _ => null
            };
        }

        private async Task<ContactResponse?> GetContactSectionAsync(Guid cvId, Guid userId, IServiceProvider serviceProvider)
        {
            var contactRepository = serviceProvider.GetRequiredService<IContactRepository>();
            var contact = await contactRepository.GetByCVIdAsync(cvId);
            if (contact == null) return null;

            return new ContactResponse
            {
                Id = contact.Id,
                CVId = contact.CVId,
                PhoneNumber = contact.PhoneNumber,
                Email = contact.Email,
                LinkedInUrl = contact.LinkedInUrl,
                GitHubUrl = contact.GitHubUrl,
                PersonalWebsiteUrl = contact.PersonalWebsiteUrl,
                Country = contact.Country,
                City = contact.City
            };
        }

        private async Task<SummaryResponse?> GetSummarySectionAsync(Guid cvId, Guid userId, IServiceProvider serviceProvider)
        {
            var summaryRepository = serviceProvider.GetRequiredService<ISummaryRepository>();
            var summary = await summaryRepository.GetByCVIdAsync(cvId);
            if (summary == null) return null;

            return new SummaryResponse
            {
                Id = summary.Id,
                CVId = summary.CVId,
                Content = summary.Content
            };
        }

        private async Task<List<EducationResponse>> GetEducationSectionAsync(Guid cvId, Guid userId, IServiceProvider serviceProvider)
        {
            var educationRepository = serviceProvider.GetRequiredService<IEducationRepository>();
            var educations = await educationRepository.GetByCVIdAndUserIdAsync(cvId, userId, new EducationQuery());
            return educations.Select(e => new EducationResponse
            {
                Id = e.Id,
                Organization = e.Organization,
                Degree = e.Degree,
                FieldOfStudy = e.FieldOfStudy,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Description = e.Description,
                Gpa = e.Gpa
            }).ToList();
        }

        private async Task<List<ExperienceResponse>> GetExperienceSectionAsync(Guid cvId, Guid userId, IServiceProvider serviceProvider)
        {
            var experienceRepository = serviceProvider.GetRequiredService<IExperienceRepository>();
            var experiences = await experienceRepository.GetByCVIdAndUserIdAsync(cvId, userId);
            return experiences.Select(e => new ExperienceResponse
            {
                Id = e.Id,
                CVId = e.CVId,
                JobTitle = e.JobTitle,
                EmploymentTypeId = e.EmploymentTypeId,
                EmploymentTypeName = e.EmploymentType?.Name ?? string.Empty,
                Organization = e.Organization,
                Location = e.Location,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Description = e.Description,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
        }

        private async Task<List<SkillResponse>> GetSkillsSectionAsync(Guid cvId, Guid userId, IServiceProvider serviceProvider)
        {
            var skillRepository = serviceProvider.GetRequiredService<ISkillRepository>();
            var skills = await skillRepository.GetByCVIdAndUserIdAsync(cvId, userId, new SkillQuery());
            return skills.Select(s => new SkillResponse
            {
                Id = s.Id,
                Category = s.Category,
                Content = s.Content,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();
        }

        private async Task<List<ProjectResponse>> GetProjectsSectionAsync(Guid cvId, Guid userId, IServiceProvider serviceProvider)
        {
            var projectRepository = serviceProvider.GetRequiredService<IProjectRepository>();
            var projects = await projectRepository.GetByCVIdAndUserIdAsync(cvId, userId, new ProjectQuery());
            return projects.Select(p => new ProjectResponse
            {
                Id = p.Id,
                CVId = p.CVId,
                Title = p.Title,
                Description = p.Description,
                Link = p.Link,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();
        }

        private async Task<List<CertificationResponse>> GetCertificationsSectionAsync(Guid cvId, Guid userId, IServiceProvider serviceProvider)
        {
            var certificationRepository = serviceProvider.GetRequiredService<ICertificationRepository>();
            var certifications = await certificationRepository.GetByCVIdAndUserIdAsync(cvId, userId, new CertificationQuery());
            return certifications.Select(c => new CertificationResponse
            {
                Id = c.Id,
                CVId = c.CVId,
                Name = c.Name,
                Organization = c.Organization,
                IssuedDate = c.IssuedDate,
                Description = c.Description
            }).ToList();
        }
    }
}
