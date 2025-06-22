using FluentAssertions;
using ServicePerfectCV.Application.DTOs.Certification.Requests;
using ServicePerfectCV.Application.DTOs.Certification.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ServicePerfectCV.IntegrationTests.Controllers
{
    public class CertificationControllerTests : IntegrationTestBase
    {
        public CertificationControllerTests(CustomWebApplicationFactory factory)
            : base(factory: factory)
        {
        }

        [Fact]
        public async Task CreateCertification_WithValidData_ReturnsOk()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            var organization = await CreateOrganization();
            AttachAccessToken(userId: user.Id);

            var request = new CreateCertificationRequest
            {
                CVId = cv.Id,
                Name = "AWS Certified Developer",
                Organization = organization.Name,
                OrganizationId = organization.Id,
                IssuedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)),
                Description = "Associate level certification for AWS development"
            };

            var response = await Client.PostAsync(requestUri: $"/api/cvs/{cv.Id}/certifications", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<CertificationResponse>(response: response);

            result.Should().NotBeNull();
            result!.Name.Should().Be(request.Name);
            result.Organization.Should().Be(request.Organization);
            result.OrganizationId.Should().Be(request.OrganizationId);
        }

        [Fact]
        public async Task CreateCertification_WithInvalidData_ReturnsBadRequest()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            AttachAccessToken(userId: user.Id);

            var request = new CreateCertificationRequest
            {
                CVId = cv.Id,
                Name = new string('A', 300),
                Organization = "Amazon Web Services"
            };

            var response = await Client.PostAsync(requestUri: $"/api/cvs/{cv.Id}/certifications", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateCertification_WithInvalidOrganizationId_ReturnsNotFound()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            AttachAccessToken(userId: user.Id);

            var request = new CreateCertificationRequest
            {
                CVId = cv.Id,
                Name = "AWS Certified Developer",
                Organization = "Amazon Web Services",
                OrganizationId = Guid.NewGuid(),
                IssuedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)),
                Description = "Associate level certification for AWS development"
            };

            var response = await Client.PostAsync(requestUri: $"/api/cvs/{cv.Id}/certifications", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateCertification_WithInvalidCVId_ReturnsNotFound()
        {
            var user = await CreateUser();
            var invalidCVId = Guid.NewGuid();
            AttachAccessToken(userId: user.Id);

            var request = new CreateCertificationRequest
            {
                CVId = invalidCVId,
                Name = "AWS Certified Developer",
                Organization = "Amazon Web Services"
            };

            var response = await Client.PostAsync(requestUri: $"/api/cvs/{invalidCVId}/certifications", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCertificationById_ExistingCertification_ReturnsOk()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            var certification = await CreateCertification(cvId: cv.Id);
            AttachAccessToken(userId: user.Id);

            var response = await Client.GetAsync(requestUri: $"/api/cvs/{cv.Id}/certifications/{certification.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<CertificationResponse>(response: response);

            result.Should().NotBeNull();
            result!.Id.Should().Be(certification.Id);
            result.Name.Should().Be(certification.Name);
            result.Organization.Should().Be(certification.Organization);
        }

        [Fact]
        public async Task GetCertificationById_NonExistingCertification_ReturnsNotFound()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            var nonExistentCertificationId = Guid.NewGuid();
            AttachAccessToken(userId: user.Id);

            var response = await Client.GetAsync(requestUri: $"/api/cvs/{cv.Id}/certifications/{nonExistentCertificationId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCertificationById_WrongUser_ReturnsNotFound()
        {
            var user1 = await CreateUser(email: "user1@example.com");
            var user2 = await CreateUser(email: "user2@example.com");
            var cv = await CreateCV(userId: user1.Id);
            var certification = await CreateCertification(cvId: cv.Id);

            AttachAccessToken(userId: user2.Id);

            var response = await Client.GetAsync(requestUri: $"/api/cvs/{cv.Id}/certifications/{certification.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAllCertifications_ReturnsListOfCertifications()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            await CreateCertification(cvId: cv.Id, name: "Certification 1");
            await CreateCertification(cvId: cv.Id, name: "Certification 2");
            await CreateCertification(cvId: cv.Id, name: "Certification 3");
            AttachAccessToken(userId: user.Id);

            var response = await Client.GetAsync(requestUri: $"/api/cvs/{cv.Id}/certifications");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<IEnumerable<CertificationResponse>>(response: response);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result!.Select(c => c.Name).Should().Contain(new[] { "Certification 1", "Certification 2", "Certification 3" });
        }

        [Fact]
        public async Task GetAllCertifications_WithQuery_ReturnsFilteredResults()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            await CreateCertification(cvId: cv.Id, name: "AWS Certification", issuedDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)));
            await CreateCertification(cvId: cv.Id, name: "Azure Certification", issuedDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2)));
            await CreateCertification(cvId: cv.Id, name: "Google Certification", issuedDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)));
            AttachAccessToken(userId: user.Id);

            var query = new CertificationQuery
            {
                Limit = 2,
                Offset = 0
            };

            var response = await Client.GetAsync(requestUri: $"/api/cvs/{cv.Id}/certifications?limit={query.Limit}&offset={query.Offset}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<IEnumerable<CertificationResponse>>(response: response);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllCertifications_WithEmptyCertifications_ReturnsEmptyList()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            AttachAccessToken(userId: user.Id);

            var response = await Client.GetAsync(requestUri: $"/api/cvs/{cv.Id}/certifications");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<IEnumerable<CertificationResponse>>(response: response);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateCertification_WithValidData_ReturnsOk()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            var organization = await CreateOrganization(name: "New Organization");
            var certification = await CreateCertification(cvId: cv.Id);
            AttachAccessToken(userId: user.Id);

            var request = new UpdateCertificationRequest
            {
                Name = "Updated Certification",
                Organization = "Updated Organization",
                OrganizationId = organization.Id,
                IssuedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                Description = "Updated description"
            };

            var response = await Client.PutAsync(requestUri: $"/api/cvs/{cv.Id}/certifications/{certification.Id}", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<CertificationResponse>(response: response);

            result.Should().NotBeNull();
            result!.Name.Should().Be(request.Name);
            result.Organization.Should().Be(request.Organization);
            result.OrganizationId.Should().Be(request.OrganizationId);
            result.Description.Should().Be(request.Description);
        }

        [Fact]
        public async Task UpdateCertification_CertificationNotFound_ReturnsNotFound()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            var nonExistentCertificationId = Guid.NewGuid();
            AttachAccessToken(userId: user.Id);

            var request = new UpdateCertificationRequest
            {
                Name = "Updated Certification",
                Organization = "Updated Organization",
                IssuedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                Description = "Updated description"
            };

            var response = await Client.PutAsync(requestUri: $"/api/cvs/{cv.Id}/certifications/{nonExistentCertificationId}", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateCertification_InvalidData_ReturnsBadRequest()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            var certification = await CreateCertification(cvId: cv.Id);
            AttachAccessToken(userId: user.Id);

            var request = new UpdateCertificationRequest
            {
                Name = new string('A', 300),
                Organization = "Updated Organization"
            };

            var response = await Client.PutAsync(requestUri: $"/api/cvs/{cv.Id}/certifications/{certification.Id}", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteCertification_ExistingCertification_ReturnsNoContent()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            var certification = await CreateCertification(cvId: cv.Id);
            AttachAccessToken(userId: user.Id);

            var response = await Client.DeleteAsync(requestUri: $"/api/cvs/{cv.Id}/certifications/{certification.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var verificationResponse = await Client.GetAsync(requestUri: $"/api/cvs/{cv.Id}/certifications/{certification.Id}");
            verificationResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCertification_NonExistingCertification_ReturnsNotFound()
        {
            var user = await CreateUser();
            var cv = await CreateCV(userId: user.Id);
            var nonExistentCertificationId = Guid.NewGuid();
            AttachAccessToken(userId: user.Id);

            var response = await Client.DeleteAsync(requestUri: $"/api/cvs/{cv.Id}/certifications/{nonExistentCertificationId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}