using FluentAssertions;
using ServicePerfectCV.Application.DTOs.Project.Requests;
using ServicePerfectCV.Application.DTOs.Project.Responses;
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
    [Collection("IntegrationTests")]
    public class ProjectControllerTests : IntegrationTestBase
    {
        public ProjectControllerTests(CustomWebApplicationFactory factory)
            : base(factory: factory)
        {
        }

        [Fact]
        public async Task CreateProject_ValidData_ReturnsOkAndCreatesProject()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var request = new CreateProjectRequest
            {
                CVId = testCV.Id,
                Title = "Portfolio Website",
                Description = "A personal portfolio website showcasing my projects and skills",
                Link = "https://portfolio.example.com",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1))
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.PostAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects",
                content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<ProjectResponse>(response: response);

            result.Should().NotBeNull();
            result!.Title.Should().Be(request.Title);
            result.Description.Should().Be(request.Description);
            result.Link.Should().Be(request.Link);

            var getResponse = await Client.GetAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var projects = await DeserializeResponse<IEnumerable<ProjectResponse>>(response: getResponse);
            projects.Should().NotBeNull();
            var projectsList = projects!.ToList();
            projectsList.Should().NotBeEmpty();
            projectsList[0].Title.Should().Be(request.Title);
        }

        [Fact]
        public async Task CreateProject_CVNotFound_ReturnsNotFound()
        {
            var testUser = await CreateUser();
            var nonExistentCVId = Guid.NewGuid();

            var request = new CreateProjectRequest
            {
                CVId = nonExistentCVId,
                Title = "Portfolio Website",
                Description = "A personal portfolio website showcasing my projects and skills",
                Link = "https://portfolio.example.com",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1))
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.PostAsync(
                requestUri: $"/api/cvs/{nonExistentCVId}/projects",
                content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
            error!.Code.Should().Be(ProjectErrors.CVNotFound.Code);
        }

        [Fact]
        public async Task UpdateProject_ValidData_ReturnsOkAndUpdatesProject()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var project = await CreateProject(
                cvId: testCV.Id,
                title: "Original Project",
                description: "Original description",
                link: "https://original.example.com",
                startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-4))
            );

            var updateRequest = new UpdateProjectRequest
            {
                Title = "Updated Project Title",
                Description = "Updated project description with more details",
                Link = "https://updated.example.com",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2))
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.PutAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects/{project.Id}",
                content: CreateJsonContent(data: updateRequest));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<ProjectResponse>(response: response);

            result.Should().NotBeNull();
            result!.Title.Should().Be(updateRequest.Title);
            result.Description.Should().Be(updateRequest.Description);
            result.Link.Should().Be(updateRequest.Link);

            var updatedProject = await ProjectRepository.GetByIdAsync(id: project.Id);
            updatedProject.Should().NotBeNull();
            updatedProject!.Title.Should().Be(updateRequest.Title);
            updatedProject.Description.Should().Be(updateRequest.Description);
        }

        [Fact]
        public async Task UpdateProject_ProjectNotFound_ReturnsNotFound()
        {
            var testUser = await CreateUser();
            var nonExistentProjectId = Guid.NewGuid();

            var updateRequest = new UpdateProjectRequest
            {
                Title = "Updated Project Title",
                Description = "Updated project description with more details",
                Link = "https://updated.example.com",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-2))
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var testCV = await CreateCV(userId: testUser.Id);
            var response = await Client.PutAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects/{nonExistentProjectId}",
                content: CreateJsonContent(data: updateRequest));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
            error!.Code.Should().Be(ProjectErrors.NotFound.Code);
        }

        [Fact]
        public async Task ListProjects_ProjectsExist_ReturnsProjects()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            await CreateProject(
                cvId: testCV.Id,
                title: "Project 1",
                description: "Description 1",
                startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-12)),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-10))
            );

            await CreateProject(
                cvId: testCV.Id,
                title: "Project 2",
                description: "Description 2",
                startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-8)),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6))
            );

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.GetAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<IEnumerable<ProjectResponse>>(response: response);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Title == "Project 1" && p.Description == "Description 1");
            result.Should().Contain(p => p.Title == "Project 2" && p.Description == "Description 2");
        }

        [Fact]
        public async Task ListProjects_WithPagination_ReturnsLimitedResults()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            for (int i = 1; i <= 15; i++)
            {
                await CreateProject(
                    cvId: testCV.Id,
                    title: $"Project {i}",
                    description: $"Description {i}"
                );
            }

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.GetAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects?limit=5&offset=5");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<IEnumerable<ProjectResponse>>(response: response);

            result.Should().NotBeNull();
            result.Should().HaveCount(5);
        }

        [Fact]
        public async Task ListProjects_WithSorting_ReturnsSortedResults()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var earliestDate = DateTime.UtcNow.AddYears(-3);
            var middleDate = DateTime.UtcNow.AddYears(-2);
            var latestDate = DateTime.UtcNow.AddYears(-1);

            await CreateProject(
                cvId: testCV.Id,
                title: "Latest Project",
                startDate: DateOnly.FromDateTime(latestDate)
            );

            await CreateProject(
                cvId: testCV.Id,
                title: "Middle Project",
                startDate: DateOnly.FromDateTime(middleDate)
            );

            await CreateProject(
                cvId: testCV.Id,
                title: "Earliest Project",
                startDate: DateOnly.FromDateTime(earliestDate)
            );

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);

            // Test ascending sort
            var ascResponse = await Client.GetAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects?sort.startDate=0");

            ascResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var ascResult = await DeserializeResponse<IEnumerable<ProjectResponse>>(response: ascResponse);
            ascResult.Should().NotBeNull();

            var ascList = ascResult!.ToList();
            ascList.Should().HaveCount(3);
            ascList[0].Title.Should().Be("Earliest Project");
            ascList[2].Title.Should().Be("Latest Project");

            // Test descending sort
            var descResponse = await Client.GetAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects?sort.startDate=1");

            descResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var descResult = await DeserializeResponse<IEnumerable<ProjectResponse>>(response: descResponse);
            descResult.Should().NotBeNull();

            var descList = descResult!.ToList();
            descList.Should().HaveCount(3);
            descList[0].Title.Should().Be("Latest Project");
            descList[2].Title.Should().Be("Earliest Project");
        }

        [Fact]
        public async Task ListProjects_NoneExist_ReturnsEmptyList()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.GetAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<IEnumerable<ProjectResponse>>(response: response);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // [Fact]
        // public async Task GetProjectById_ProjectExists_ReturnsProject()
        // {
        //     var testUser = await CreateUser();
        //     var testCV = await CreateCV(userId: testUser.Id);
        //
        //     var project = await CreateProject(
        //         cvId: testCV.Id,
        //         title: "Sample Project",
        //         description: "A sample project description",
        //         link: "https://sample.example.com"
        //     );
        //
        //     AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
        //     var response = await Client.GetAsync(
        //         requestUri: $"/api/cvs/{testCV.Id}/projects/{project.Id}");
        //
        //     response.StatusCode.Should().Be(HttpStatusCode.OK);
        //     var result = await DeserializeResponse<ProjectResponse>(response: response);
        //
        //     result.Should().NotBeNull();
        //     result!.Id.Should().Be(project.Id);
        //     result.Title.Should().Be(project.Title);
        //     result.Description.Should().Be(project.Description);
        //     result.Link.Should().Be(project.Link);
        // }

        [Fact]
        public async Task GetProjectById_ProjectNotFound_ReturnsNotFound()
        {
            var testUser = await CreateUser();
            var nonExistentId = Guid.NewGuid();

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var testCV = await CreateCV(userId: testUser.Id);
            var response = await Client.GetAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects/{nonExistentId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
            error!.Code.Should().Be(ProjectErrors.NotFound.Code);
        }

        [Fact]
        public async Task CreateProject_InvalidData_ReturnsBadRequest()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var request = new CreateProjectRequest
            {
                CVId = testCV.Id,
                Title = "", // Empty title, should fail validation
                Description = "Valid description",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1))
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.PostAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects",
                content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateProject_InvalidData_ReturnsBadRequest()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var project = await CreateProject(
                cvId: testCV.Id,
                startDate: DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow)
            );

            var updateRequest = new UpdateProjectRequest
            {
                Title = "Valid Title",
                Description = "Valid Description",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2)) // End date before start date
            };

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.PutAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects/{project.Id}",
                content: CreateJsonContent(data: updateRequest));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteProject_ProjectExists_ReturnsNoContent()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var project = await CreateProject(
                cvId: testCV.Id,
                title: "Project to Delete",
                description: "This project will be deleted"
            );

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.DeleteAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects/{project.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await Client.GetAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects/{project.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteProject_ProjectNotFound_ReturnsNotFound()
        {
            var testUser = await CreateUser();
            var nonExistentProjectId = Guid.NewGuid();

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var testCV = await CreateCV(userId: testUser.Id);
            var response = await Client.DeleteAsync(
                requestUri: $"/api/cvs/{testCV.Id}/projects/{nonExistentProjectId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);

            error.Should().NotBeNull();
            error!.Code.Should().Be(ProjectErrors.NotFound.Code);
        }

        [Fact]
        public async Task GetProjectByCVId_UserNotHavingCV_ReturnsNotFound()
        {
            var userA = await CreateUser(email: "owner@example.com");
            var userB = await CreateUser(email: "other@example.com");
            var cvA = await CreateCV(userId: userA.Id);

            var project = await CreateProject(
                cvId: cvA.Id,
                title: "Test Project",
                description: "Test Description"
            );

            AttachAccessToken(userId: userB.Id, userRole: userB.Role);
            var response = await Client.GetAsync(
                requestUri: $"/api/cvs/{cvA.Id}/projects");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<IEnumerable<ProjectResponse>>(response: response);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}