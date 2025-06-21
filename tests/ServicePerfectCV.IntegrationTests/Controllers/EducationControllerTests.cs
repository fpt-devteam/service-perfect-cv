using System.Net;
using System.Threading.Tasks;
using ServicePerfectCV.Application.DTOs.Education.Requests;
using ServicePerfectCV.Application.DTOs.Education.Responses;
using ServicePerfectCV.Application.DTOs.Pagination.Requests;
using ServicePerfectCV.Application.DTOs.Pagination.Responses;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using ServicePerfectCV.Domain.Constants;
using ServicePerfectCV.Domain.Entities;
using Xunit.Abstractions;

namespace ServicePerfectCV.IntegrationTests.Controllers
{
    public class EducationControllerTests : IntegrationTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EducationControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(factory)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task CreateEducation_WithValidData_ReturnsCreated()
        {
            // Arrange
            var user = await CreateUser();
            var cv = await CreateCV(user.Id);
            AttachAccessToken(user.Id);

            var request = new CreateEducationRequest
            {
                CVId = cv.Id,
                Degree = "Master of Science",
                Organization = "Tech University",
                FieldOfStudy = "Computer Engineering",
                StartDate = new System.DateOnly(2020, 9, 1),
                EndDate = new System.DateOnly(2022, 6, 1),
                Description = "Focused on AI and ML.",
                Gpa = 3.9m
            };

            // Act
            var response = await Client.PostAsync($"/api/cvs/{cv.Id}/educations", CreateJsonContent(request));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var createdEducation = await DeserializeResponse<EducationResponse>(response);
            Assert.NotNull(createdEducation);
            Assert.Equal(request.Degree, createdEducation.Degree);
        }

        [Fact]
        public async Task GetEducation_WithValidId_ReturnsEducation()
        {
            // Arrange
            var user = await CreateUser();
            var cv = await CreateCV(user.Id);
            var education = await CreateEducation(cv.Id);
            AttachAccessToken(user.Id);

            // Act
            var response = await Client.GetAsync($"/api/cvs/{cv.Id}/educations/{education.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var fetchedEducation = await DeserializeResponse<EducationResponse>(response);
            Assert.NotNull(fetchedEducation);
            Assert.Equal(education.Degree, fetchedEducation.Degree);
        }

        [Fact]
        public async Task ListEducations_ReturnsListOfEducations()
        {
            // Arrange
            var user = await CreateUser();
            var cv = await CreateCV(user.Id);
            await CreateEducation(cv.Id, degreeName: "Education 1");
            await CreateEducation(cv.Id, degreeName: "Education 2");
            await CreateEducation(cv.Id, degreeName: "Education 3");
            await CreateEducation(cv.Id, degreeName: "Education 4");
            AttachAccessToken(user.Id);

            // Act
            var response = await Client.GetAsync($"/api/cvs/{cv.Id}/educations");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var educations = await DeserializeResponse<IEnumerable<EducationResponse>>(response);
            Assert.NotNull(educations);
            Assert.Equal(4, educations.Count());
        }
        
        [Fact]
        public async Task GetEducations_WithPagination_ReturnsPagedResults()
        {
            // Arrange
            var user = await CreateUser();
            var cv = await CreateCV(user.Id);
            AttachAccessToken(user.Id);
            
            // Create multiple education records
            // Create multiple education records
            await CreateEducation(cv.Id, degreeName: "Degree 0", startDate: DateOnly.Parse("2020-01-01"));
            await CreateEducation(cv.Id, degreeName: "Degree 1", startDate: DateOnly.Parse("2021-01-02"));
            await CreateEducation(cv.Id, degreeName: "Degree 2", startDate: DateOnly.Parse("2022-01-03"));
            await CreateEducation(cv.Id, degreeName: "Degree 3", startDate: DateOnly.Parse("2023-01-04"));

            var query = new EducationQuery()
            {
                Limit = 3,
                Offset = 0,
                Sort = new EducationSort
                {
                    StartDate = SortOrder.Descending
                }
            };

            // Act
            var response = await Client.GetAsync($"/api/cvs/{cv.Id}/educations?limit={query.Limit}&offset={query.Offset}&Sort.StartDate={query.Sort.StartDate}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await DeserializeResponse<IEnumerable<EducationResponse>>(response);
            
            Assert.NotNull(result);
            var educationResponses = result.ToList();
            Assert.Equal(query.Limit, educationResponses.Count());
            Assert.Equal("Degree 3", educationResponses[0].Degree);
        }

        [Fact]
        public async Task UpdateEducation_WithValidData_ReturnsOk()
        {
            // Arrange
            var user = await CreateUser();
            var cv = await CreateCV(user.Id);
            var education = await CreateEducation(cv.Id);
            AttachAccessToken(user.Id);

            var request = new UpdateEducationRequest
            {
                Degree = "Updated Degree",
                Organization = "Updated University",
                FieldOfStudy = "Updated Field",
                StartDate = new System.DateOnly(2021, 1, 1),
                EndDate = new System.DateOnly(2023, 1, 1)
            };

            // Act
            var response = await Client.PutAsync($"/api/cvs/{cv.Id}/educations/{education.Id}", CreateJsonContent(request));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updatedEducation = await DeserializeResponse<EducationResponse>(response);
            Assert.NotNull(updatedEducation);
            Assert.Equal(request.Degree, updatedEducation.Degree);
        }

        [Fact]
        public async Task DeleteEducation_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var user = await CreateUser();
            var cv = await CreateCV(user.Id);
            var education = await CreateEducation(cv.Id);
            AttachAccessToken(user.Id);

            // Act
            var response = await Client.DeleteAsync($"/api/cvs/{cv.Id}/educations/{education.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}