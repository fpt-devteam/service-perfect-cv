using FluentAssertions;
using ServicePerfectCV.Application.DTOs.Experience.Requests;
using ServicePerfectCV.Application.DTOs.Experience.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ServicePerfectCV.IntegrationTests.Controllers
{
    public class ExperienceControllerTests : IntegrationTestBase
    {
        public ExperienceControllerTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task CreateExperience_ValidData_ReturnsOkAndCreatesExperience()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);
            var employmentType = await CreateEmploymentType();

            var request = new CreateExperienceRequest
            {
                CVId = testCV.Id,
                JobTitle = "Software Engineer",
                EmploymentTypeId = employmentType.Id,
                Company = "Tech Corp",
                Location = "San Francisco, CA",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)),
                Description = "Developed applications"
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.PostAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<ExperienceResponse>(response: response);

            result.Should().NotBeNull();
            result!.JobTitle.Should().Be(request.JobTitle);
            result.Company.Should().Be(request.Company);
            result.Location.Should().Be(request.Location);

            var getResponse = await Client.GetAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var experiences = await DeserializeResponse<IEnumerable<ExperienceResponse>>(response: getResponse);
            experiences.Should().NotBeNull();
            var experiencesList = experiences!.ToList();
            experiencesList.Should().NotBeEmpty();
            experiencesList[0].JobTitle.Should().Be(request.JobTitle);
        }

        [Fact]
        public async Task CreateExperience_CVNotFound_ReturnsNotFound()
        {
            var testUser = await CreateUser();
            var nonExistentCVId = Guid.NewGuid();
            var employmentType = await CreateEmploymentType();
            var jobTitle = await CreateJobTitle();
            var company = await CreateCompany();

            var request = new CreateExperienceRequest
            {
                CVId = nonExistentCVId,
                JobTitle = jobTitle.Name,
                EmploymentTypeId = employmentType.Id,
                JobTitleId = jobTitle.Id,
                CompanyId = company.Id,
                Company = "Tech Corp",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6))
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.PostAsync(requestUri: $"/api/cvs/{nonExistentCVId}/experiences", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
            error!.Code.Should().Be(ExperienceErrors.CVNotFound.Code);
        }

        [Fact]
        public async Task UpdateExperience_ValidData_ReturnsOkAndUpdatesExperience()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);
            var employmentType = await CreateEmploymentType();
            var jobTitle = await CreateJobTitle("Junior Developer");
            var company = await CreateCompany("Old Corp");

            var experience = new Experience
            {
                Id = Guid.NewGuid(),
                CVId = testCV.Id,
                JobTitle = "Junior Developer",
                EmploymentTypeId = employmentType.Id,
                JobTitleId = jobTitle.Id,
                CompanyId = company.Id,
                Company = "Old Corp",
                Location = "Chicago, IL",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await ExperienceRepository.CreateAsync(entity: experience);
            await ExperienceRepository.SaveChangesAsync();

            var updateRequest = new UpdateExperienceRequest
            {
                JobTitle = "Senior Developer",
                EmploymentTypeId = experience.EmploymentTypeId,
                Company = "New Corp",
                Location = "Remote",
                Description = "Lead developer for enterprise applications",
                JobTitleId = experience.JobTitleId,
                CompanyId = experience.CompanyId,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6))
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.PutAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences/{experience.Id}", content: CreateJsonContent(data: updateRequest));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<ExperienceResponse>(response: response);

            result.Should().NotBeNull();
            result!.JobTitle.Should().Be(updateRequest.JobTitle);
            result.Company.Should().Be(updateRequest.Company);
            result.Location.Should().Be("Remote");

            var updatedExperience = await ExperienceRepository.GetByIdAsync(id: experience.Id);
            updatedExperience.Should().NotBeNull();
            updatedExperience!.JobTitle.Should().Be(updateRequest.JobTitle);
            updatedExperience.Company.Should().Be(updateRequest.Company);
        }

        [Fact]
        public async Task UpdateExperience_ExperienceNotFound_ReturnsNotFound()
        {
            var testUser = await CreateUser();
            var nonExistentExperienceId = Guid.NewGuid();
            var employmentType = await CreateEmploymentType();
            var jobTitle = await CreateJobTitle();
            var company = await CreateCompany();

            var updateRequest = new UpdateExperienceRequest
            {
                JobTitle = "Senior Developer",
                Company = "New Corp",
                Location = "Remote",
                Description = "Lead developer position",
                EmploymentTypeId = employmentType.Id,
                JobTitleId = jobTitle.Id,
                CompanyId = company.Id,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6))
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var testCV = await CreateCV(userId: testUser.Id);
            var response = await Client.PutAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences/{nonExistentExperienceId}", content: CreateJsonContent(data: updateRequest));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
            error!.Code.Should().Be(ExperienceErrors.NotFound.Code);
        }

        [Fact]
        public async Task GetExperienceByCVId_ExperiencesExist_ReturnsExperiences()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            await CreateExperience(
                cvId: testCV.Id,
                jobTitle: "Developer 1",
                company: "Company 1",
                startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5)),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3))
            );

            await CreateExperience(
                cvId: testCV.Id,
                jobTitle: "Developer 2",
                company: "Company 2",
                startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3)),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))
            );

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.GetAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<IEnumerable<ExperienceResponse>>(response: response);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(e => e.JobTitle == "Developer 1" && e.Company == "Company 1");
            result.Should().Contain(e => e.JobTitle == "Developer 2" && e.Company == "Company 2");
        }

        [Fact]
        public async Task GetExperienceByCVId_NoneExist_ReturnsEmptyList()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.GetAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<IEnumerable<ExperienceResponse>>(response: response);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetExperienceById_ExperienceExists_ReturnsExperience()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var experience = await CreateExperience(
                cvId: testCV.Id,
                jobTitle: "Senior Engineer",
                company: "Tech Company",
                startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5)),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2))
            );

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.GetAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences/{experience.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<ExperienceResponse>(response: response);

            result.Should().NotBeNull();
            result!.Id.Should().Be(experience.Id);
            result.JobTitle.Should().Be(experience.JobTitle);
            result.Company.Should().Be(experience.Company);
        }

        [Fact]
        public async Task GetExperienceById_ExperienceNotFound_ReturnsNotFound()
        {
            var testUser = await CreateUser();
            var nonExistentId = Guid.NewGuid();

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var testCV = await CreateCV(userId: testUser.Id);
            var response = await Client.GetAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences/{nonExistentId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
            error!.Code.Should().Be(ExperienceErrors.NotFound.Code);
        }

        [Fact]
        public async Task CreateExperience_InvalidData_ReturnsBadRequest()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var request = new CreateExperienceRequest
            {
                CVId = testCV.Id,
                JobTitle = "Test Job Title",
                EmploymentTypeId = Guid.NewGuid(),
                Company = "me that exceeds the maximum allowed length",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-3))
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.PostAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateExperience_InvalidData_ReturnsBadRequest()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var experience = await CreateExperience(
                cvId: testCV.Id,
                startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))
            );

            var updateRequest = new UpdateExperienceRequest
            {
                JobTitle = "Senior Developer",
                EmploymentTypeId = experience.EmploymentTypeId,
                Company = "New Corp",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2)) // End date before start date
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.PutAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences/{experience.Id}", content: CreateJsonContent(data: updateRequest));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetExperienceByCVId_CVNotFound_ReturnsNotFound()
        {
            var testUser = await CreateUser();
            var nonExistentCVId = Guid.NewGuid();

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.GetAsync(requestUri: $"/api/cvs/{nonExistentCVId}/experiences");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
            error!.Code.Should().Be(ExperienceErrors.NotFound.Code);
        }

        [Fact]
        public async Task GetExperienceByCVId_UserNotHavingCV_ReturnsNotFound()
        {
            var testUser = await CreateUser();
            var otherUserId = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var experience = await CreateExperience(
                cvId: testCV.Id,
                jobTitle: "Senior Engineer",
                company: "Tech Company",
                startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5)),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-2))
            );

            AttachAccessToken(userId: otherUserId.Id, userRole: testUser.Role);
            var response = await Client.GetAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
            error!.Code.Should().Be(ExperienceErrors.NotFound.Code);
        }

        [Fact]
        public async Task DeleteExperience_ExperienceExists_ReturnsNoContent()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var experience = await CreateExperience(
                cvId: testCV.Id,
                jobTitle: "Test Job",
                company: "Test Company"
            );

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.DeleteAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences/{experience.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await Client.GetAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences/{experience.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteExperience_ExperienceNotFound_ReturnsNotFound()
        {
            var testUser = await CreateUser();
            var nonExistentExperienceId = Guid.NewGuid();

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var testCV = await CreateCV(userId: testUser.Id);
            var response = await Client.DeleteAsync(requestUri: $"/api/cvs/{testCV.Id}/experiences/{nonExistentExperienceId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
            error!.Code.Should().Be(ExperienceErrors.NotFound.Code);
        }
    }
}
