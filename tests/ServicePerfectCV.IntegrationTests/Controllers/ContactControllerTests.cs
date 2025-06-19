using FluentAssertions;
using ServicePerfectCV.Application.DTOs.Contact.Requests;
using ServicePerfectCV.Application.DTOs.Contact.Responses;
using ServicePerfectCV.Application.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ServicePerfectCV.IntegrationTests.Controllers
{
    public class ContactControllerTests : IntegrationTestBase
    {
        public ContactControllerTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task UpsertContact_ContactDoesNotExist_CreateNewContact()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var request = new UpsertContactRequest
            {
                CVId = testCV.Id,
                Email = "contact@example.com",
                PhoneNumber = "+1234567890",
                Country = "United States",
                City = "New York"
            };
            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response =
                await Client.PostAsync(requestUri: "/api/contacts", content: CreateJsonContent(data: request));

            await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await DeserializeResponse<ContactResponse>(response: response);

            result.Should().NotBeNull();
            result!.Email.Should().Be(request.Email);
            result.PhoneNumber.Should().Be(request.PhoneNumber);
            result.Country.Should().Be(request.Country);
            result.City.Should().Be(request.City);

            var contact = await ContactRepository.GetByCVIdAsync(cvId: testCV.Id);
            contact.Should().NotBeNull();
            contact!.Email.Should().Be(request.Email);
        }

        [Fact]
        public async Task UpsertContact_ProvidingOnlySomeFields_UpdateContact()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var initialContact = await CreateContact(
                cvId: testCV.Id,
                email: "initial@example.com",
                phone: "+0987654321"
                );

            initialContact.Country = "Initial Country";
            initialContact.City = "Initial City";
            ContactRepository.Update(entity: initialContact);
            await ContactRepository.SaveChangesAsync();

            var updateRequest = new UpsertContactRequest
            {
                CVId = testCV.Id,
                Email = "updated@example.com",
            };
            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);

            var updateResponse = await Client.PostAsync(requestUri: "/api/contacts",
                content: CreateJsonContent(data: updateRequest));

            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await DeserializeResponse<ContactResponse>(response: updateResponse);

            result.Should().NotBeNull();
            result!.Email.Should().Be(updateRequest.Email);
            result.PhoneNumber.Should().Be(initialContact.PhoneNumber);
            result.Country.Should().Be(initialContact.Country);
            result.City.Should().Be(initialContact.City);

            var updatedContact = await ContactRepository.GetByCVIdAsync(cvId: testCV.Id);
            updatedContact.Should().NotBeNull();
            updatedContact!.Email.Should().Be(updateRequest.Email);
            updatedContact.PhoneNumber.Should().Be(initialContact.PhoneNumber);
            updatedContact.Country.Should().Be(initialContact.Country);
            updatedContact.City.Should().Be(initialContact.City);
        }

        [Fact]
        public async Task UpsertContact_ContactExists_UpdateContact()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id, title: "Update Test CV");

            var existingContact = await CreateContact(
                cvId: testCV.Id,
                email: "existing@example.com",
                phone: "+1234567890");

            var updateRequest = new UpsertContactRequest
            {
                CVId = testCV.Id,
                Email = "updated@example.com",
            };
            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);

            var response = await Client.PostAsync(requestUri: "/api/contacts",
                content: CreateJsonContent(data: updateRequest));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedContact = await ContactRepository.GetByCVIdAsync(cvId: testCV.Id);
            updatedContact.Should().NotBeNull();
            updatedContact!.Email.Should().Be(updateRequest.Email);
            updatedContact.PhoneNumber.Should().Be(existingContact.PhoneNumber);
        }

        [Fact]
        public async Task GetContactByCVId_ContactExists_ReturnContact()
        {
            var testUser = await CreateUser();
            var testCV = await CreateCV(userId: testUser.Id);

            var initialContact = await CreateContact(
                cvId: testCV.Id,
                email: "initial@example.com",
                phone: "+0987654321");

            initialContact.Country = "Initial Country";
            initialContact.City = "Initial City";
            ContactRepository.Update(entity: initialContact);
            await ContactRepository.SaveChangesAsync();

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);

            var response = await Client.GetAsync(requestUri: $"/api/contacts/cv/{testCV.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await DeserializeResponse<ContactResponse>(response: response);

            content.Should().NotBeNull();
            content!.CVId.Should().Be(initialContact.CVId);
            content.Email.Should().Be(initialContact.Email);
        }

        [Fact]
        public async Task GetContactByCVId_ContactDoesNotExist_ReturnNotFound()
        {
            var testUser = await CreateUser(email: "no-contact@example.com");
            await CreateCV(userId: testUser.Id, title: "CV Without Contact");

            var nonExistentCVId = Guid.NewGuid();

            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response = await Client.GetAsync(requestUri: $"/api/contacts/cv/{nonExistentCVId}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpsertContact_CVDoesNotExist_ReturnNotFound()
        {
            var nonExistentCVId = Guid.NewGuid();
            var request = new UpsertContactRequest
            {
                CVId = nonExistentCVId,
                Email = "test@example.com"
            };
            AttachAccessToken(userId: Guid.Empty);
            var response =
                await Client.PostAsync(requestUri: "/api/contacts", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var error = await DeserializeResponse<Error>(response: response);
            error.Should().NotBeNull();
            error!.Code.Should().Be(ContactErrors.CVNotFound.Code);
            error.Message.Should().Be(ContactErrors.CVNotFound.Message);
        }

        [Fact]
        public async Task UpsertContact_InvalidDataSubmitted_ReturnBadRequest()
        {
            var testUser = await CreateUser(email: "validation-test@example.com");
            var testCV = await CreateCV(userId: testUser.Id, title: "Validation Test CV");

            var request = new UpsertContactRequest
            {
                CVId = testCV.Id,
                PhoneNumber = "+098765432100812439840",
            };
            AttachAccessToken(userId: testUser.Id, userRole: testUser.Role);
            var response =
                await Client.PostAsync(requestUri: "/api/contacts", content: CreateJsonContent(data: request));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpsertContact_MissingRequiredProperty_ReturnBadRequest()
        {
            var invalidRequest = new
            {
                Email = "test@example.com",
                PhoneNumber = "+1234567890"
            };
            AttachAccessToken(userId: Guid.Empty);

            var response = await Client.PostAsync(requestUri: "/api/contacts",
                content: CreateJsonContent(data: invalidRequest));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}