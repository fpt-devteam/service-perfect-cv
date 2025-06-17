# Contact API Integration Tests

This project contains integration tests for the Contact API, testing the end-to-end functionality including HTTP requests, controller actions, service processing, and data access.

## Project Structure

- **CustomWebApplicationFactory.cs**: Configures the test environment with in-memory database and test authentication
- **IntegrationTestBase.cs**: Base class with common testing utilities for all test classes
- **Helpers/**
  - **TestAuthHandler.cs**: Handles authentication for tests without requiring real tokens
  - **TestDataFactory.cs**: Creates test data consistently across test cases
- **Controllers/**
  - **ContactControllerTests.cs**: Tests for the main functionality of the Contact API
  - **ContactErrorHandlingTests.cs**: Tests for error handling scenarios
- **Models/**
  - **ErrorResponse.cs**: Model for deserializing API error responses

## Running the Tests

To run all tests:
```
dotnet test tests/ServicePerfectCV.IntegrationTests
```

To run specific test(s):
```
dotnet test --filter "FullyQualifiedName~ServicePerfectCV.IntegrationTests.Controllers.ContactControllerTests"
```

## Test Approach

These integration tests verify that:

1. The Contact API correctly handles CRUD operations:
   - Creating new contacts when they don't exist
   - Updating contacts when they exist
   - Reading contacts by CV ID

2. Error handling works properly:
   - Returns appropriate status codes for errors
   - Returns structured error responses
   - Validates request data properly

3. Authentication and authorization work:
   - Endpoints require authentication
   - Unauthenticated requests are rejected

## In-Memory Database

Tests use an in-memory database to avoid affecting any real database. The database is seeded with test data in the `CustomWebApplicationFactory` including:

- A test user with ID: `11111111-1111-1111-1111-111111111111`
- A test CV with ID: `22222222-2222-2222-2222-222222222222`
- A CV with existing contact with ID: `33333333-3333-3333-3333-333333333333`

## Best Practices Used

1. **Isolated Test Environment**: Each test runs in an isolated environment
2. **In-Memory Database**: No external dependencies required for testing
3. **Authentication Simulation**: Tests authorized endpoints without real tokens
4. **Base Test Class**: Reduces code duplication in test classes
5. **Test Data Factory**: Creates consistent test data
6. **Fluent Assertions**: Makes test assertions more readable
7. **Multiple Test Cases**: Tests various scenarios including error conditions
