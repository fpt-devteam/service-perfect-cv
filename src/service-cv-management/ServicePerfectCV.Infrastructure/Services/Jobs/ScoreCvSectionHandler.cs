using Microsoft.Extensions.Logging;
using ServicePerfectCV.Application.DTOs.AI;
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
using ServicePerfectCV.Application.Services.Jobs;
using ServicePerfectCV.Domain.Entities;
using ServicePerfectCV.Domain.Enums;
using ServicePerfectCV.Domain.ValueObjects;
using ServicePerfectCV.Infrastructure.Services.AI.SemanticKernel;
using System.Text.Json;

namespace ServicePerfectCV.Infrastructure.Services.Jobs
{
    public sealed class ScoreCvSectionHandler : IJobHandler
    {
        private readonly SectionScoreService _sectionScoreService;
        private readonly IJobDescriptionRepository _jobDescriptionRepository;
        private readonly IContactRepository _contactRepository;
        private readonly ISummaryRepository _summaryRepository;
        private readonly IEducationRepository _educationRepository;
        private readonly IExperienceRepository _experienceRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ICertificationRepository _certificationRepository;
        private readonly ISectionScoreResultRepository _sectionScoreResultRepository;
        private readonly IObjectHasher _objectHasher;
        private readonly IJsonHelper _jsonHelper;
        private readonly ILogger<ScoreCvSectionHandler> _logger;

        public JobType JobType => JobType.ScoreCV;

        public ScoreCvSectionHandler(
            SectionScoreService sectionScoreService,
            IJobDescriptionRepository jobDescriptionRepository,
            IContactRepository contactRepository,
            ISummaryRepository summaryRepository,
            IEducationRepository educationRepository,
            IExperienceRepository experienceRepository,
            ISkillRepository skillRepository,
            IProjectRepository projectRepository,
            ICertificationRepository certificationRepository,
            ISectionScoreResultRepository sectionScoreResultRepository,
            IObjectHasher objectHasher,
            IJsonHelper jsonHelper,
            ILogger<ScoreCvSectionHandler> logger)
        {
            _sectionScoreService = sectionScoreService;
            _jobDescriptionRepository = jobDescriptionRepository;
            _contactRepository = contactRepository;
            _summaryRepository = summaryRepository;
            _educationRepository = educationRepository;
            _experienceRepository = experienceRepository;
            _skillRepository = skillRepository;
            _projectRepository = projectRepository;
            _certificationRepository = certificationRepository;
            _sectionScoreResultRepository = sectionScoreResultRepository;
            _objectHasher = objectHasher;
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

                // Step 1: Get JobDescription by CvId
                var jobDescription = await _jobDescriptionRepository.GetByCVIdAsync(scoreSectionInputDto.CvId);
                if (jobDescription?.SectionRubrics == null)
                {
                    return JobHandlerResult.Failure("score_cv.no_rubric", "No section rubric found for CV",
                        _jsonHelper.SerializeToDocument(allScoreResults));
                }

                // Step 2: Load existing CvSectionScoreResults
                var existingResults = await _sectionScoreResultRepository.GetByCVIdAsync(scoreSectionInputDto.CvId);
                var existingResultsDict = existingResults.ToDictionary(r => r.SectionType, r => r);

                // Step 3: Process each section
                foreach (var sectionType in Enum.GetValues<SectionType>())
                {
                    // Check if rubric exists for this section
                    if (!jobDescription.SectionRubrics.TryGetValue(sectionType, out var sectionRubric))
                        continue;

                    try
                    {
                        var sectionContent = await GetSectionContentAsync(sectionType, scoreSectionInputDto.CvId, scoreSectionInputDto.UserId);
                        if (sectionContent == null)
                            continue;

                        // Compute hashes
                        var jdHash = _objectHasher.Hash(jobDescription);
                        var sectionContentHash = _objectHasher.Hash(sectionContent);

                        // Check if we need to update (hash differs)
                        var needsUpdate = true;
                        if (existingResultsDict.TryGetValue(sectionType, out var existingResult))
                        {
                            needsUpdate = existingResult.JdHash != jdHash ||
                                        existingResult.SectionContentHash != sectionContentHash;
                        }

                        SectionScore scoreResult;

                        if (needsUpdate)
                        {
                            // Convert section content to string and rubric to string
                            var sectionContentString = _jsonHelper.Serialize(sectionContent);
                            var sectionRubricString = _jsonHelper.Serialize(sectionRubric);

                            // Prompt AI with JobDescription and SectionContent
                            scoreResult = await _sectionScoreService.ScoreSectionAsync(
                                sectionRubricString, sectionContentString, sectionType.ToString(), cancellationToken);

                            // Update the weight from the rubric
                            scoreResult.Weight0To1 = sectionRubric.Weight0To1;

                            // Upsert to database
                            var sectionScoreResultEntity = new SectionScoreResult
                            {
                                Id = existingResult?.Id ?? Guid.NewGuid(),
                                CVId = scoreSectionInputDto.CvId,
                                SectionType = sectionType,
                                JdHash = jdHash,
                                SectionContentHash = sectionContentHash,
                                SectionScore = scoreResult,
                                CreatedAt = existingResult?.CreatedAt ?? DateTimeOffset.UtcNow,
                                UpdatedAt = DateTimeOffset.UtcNow
                            };

                            if (existingResult != null)
                            {
                                _sectionScoreResultRepository.Update(sectionScoreResultEntity);
                            }
                            else
                            {
                                await _sectionScoreResultRepository.CreateAsync(sectionScoreResultEntity);
                            }

                            await _sectionScoreResultRepository.SaveChangesAsync();
                        }
                        else
                        {
                            // Use existing cached result
                            scoreResult = existingResult!.SectionScore;
                        }

                        allScoreResults[sectionType] = scoreResult;
                    }
                    catch (Exception)
                    {
                        // Log section-specific error but continue with other sections
                        // Add empty result for this section
                        allScoreResults[sectionType] = new SectionScore
                        {
                            CriteriaScores = new List<CriteriaScore>(),
                            TotalScore0To5 = 0,
                            Weight0To1 = sectionRubric.Weight0To1
                        };
                    }
                }

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
                return JobHandlerResult.Failure("score_cv.error", ex.Message,
                    _jsonHelper.SerializeToDocument(allScoreResults));
            }
        }

        private async Task<ContactResponse?> GetContactSectionAsync(Guid cvId, Guid userId)
        {
            var contact = await _contactRepository.GetByCVIdAsync(cvId);
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

        private async Task<SummaryResponse?> GetSummarySectionAsync(Guid cvId, Guid userId)
        {
            var summary = await _summaryRepository.GetByCVIdAsync(cvId);
            if (summary == null) return null;

            return new SummaryResponse
            {
                Id = summary.Id,
                CVId = summary.CVId,
                Content = summary.Content
            };
        }

        private async Task<List<EducationResponse>> GetEducationSectionAsync(Guid cvId, Guid userId)
        {
            var educations = await _educationRepository.GetByCVIdAndUserIdAsync(cvId, userId, new EducationQuery());
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

        private async Task<List<ExperienceResponse>> GetExperienceSectionAsync(Guid cvId, Guid userId)
        {
            var experiences = await _experienceRepository.GetByCVIdAndUserIdAsync(cvId, userId);
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

        private async Task<List<SkillResponse>> GetSkillsSectionAsync(Guid cvId, Guid userId)
        {
            var skills = await _skillRepository.GetByCVIdAndUserIdAsync(cvId, userId, new SkillQuery());
            return skills.Select(s => new SkillResponse
            {
                Id = s.Id,
                Category = s.Category,
                Content = s.Content,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();
        }

        private async Task<List<ProjectResponse>> GetProjectsSectionAsync(Guid cvId, Guid userId)
        {
            var projects = await _projectRepository.GetByCVIdAndUserIdAsync(cvId, userId, new ProjectQuery());
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

        private async Task<List<CertificationResponse>> GetCertificationsSectionAsync(Guid cvId, Guid userId)
        {
            var certifications = await _certificationRepository.GetByCVIdAndUserIdAsync(cvId, userId, new CertificationQuery());
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

        private async Task<object?> GetSectionContentAsync(SectionType sectionType, Guid cvId, Guid userId)
        {
            return sectionType switch
            {
                SectionType.Contact => await GetContactSectionAsync(cvId, userId),
                SectionType.Summary => await GetSummarySectionAsync(cvId, userId),
                SectionType.Education => await GetEducationSectionAsync(cvId, userId),
                SectionType.Experience => await GetExperienceSectionAsync(cvId, userId),
                SectionType.Skills => await GetSkillsSectionAsync(cvId, userId),
                SectionType.Projects => await GetProjectsSectionAsync(cvId, userId),
                SectionType.Certifications => await GetCertificationsSectionAsync(cvId, userId),
                _ => null
            };
        }
    }
}
