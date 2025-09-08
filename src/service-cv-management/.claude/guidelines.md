# ServicePerfectCV Codebase Guidelines

## Project Overview

ServicePerfectCV is a .NET 8 web API application built with Clean Architecture principles, consisting of four main layers:
- **ServicePerfectCV.Domain** - Core business entities and rules
- **ServicePerfectCV.Application** - Application services and business logic
- **ServicePerfectCV.Infrastructure** - Data access and external services
- **ServicePerfectCV.WebApi** - REST API controllers and presentation layer

## Code Organization and Package Structure

### 1. ServicePerfectCV.Domain
```
ServicePerfectCV.Domain/
├── Common/              # Base interfaces and common types
├── Constants/          # Application constants and static values
├── Constraints/        # Domain validation constraints
├── Entities/          # Core business entities
├── Enums/            # Domain enumerations
└── ValueObjects/     # Domain value objects
```

**Purpose**: Contains core business logic, entities, and domain rules. No dependencies on other layers.

### 2. ServicePerfectCV.Application
```
ServicePerfectCV.Application/
├── Configurations/    # Application configuration classes
├── DTOs/             # Data Transfer Objects
│   ├── [Entity]/
│   │   ├── Requests/  # Request DTOs
│   │   └── Responses/ # Response DTOs
│   └── Pagination/    # Pagination-related DTOs
├── Exceptions/       # Custom application exceptions
├── Interfaces/       # Repository and service interfaces
├── Mappings/         # AutoMapper profiles
└── Services/         # Application business services
```

**Purpose**: Contains application-specific business logic, DTOs, and service interfaces.

### 3. ServicePerfectCV.Infrastructure
```
ServicePerfectCV.Infrastructure/
├── Data/
│   ├── Configurations/  # Entity Framework configurations
│   └── Seeding/        # Database seeding logic
├── Helpers/            # Utility helpers
├── Migrations/         # Entity Framework migrations
├── Repositories/       # Repository implementations
│   └── Common/         # Base repository classes
└── Services/           # Infrastructure service implementations
```

**Purpose**: Implements data access, external services, and infrastructure concerns.

### 4. ServicePerfectCV.WebApi
```
ServicePerfectCV.WebApi/
├── Controllers/        # REST API controllers
├── Extensions/         # Service collection extensions
├── Middlewares/        # Custom middleware classes
└── Templates/          # Static template files
```

**Purpose**: Handles HTTP requests, API endpoints, and presentation logic.

## Coding Conventions

### 1. Naming Conventions

#### Classes and Methods
- **PascalCase** for classes, methods, properties, and public members
- **camelCase** for parameters and local variables
- **Descriptive names** that clearly indicate purpose

```csharp
// ✅ Good
public class OrganizationService
public async Task<IEnumerable<OrganizationSuggestionResponse>> GetSuggestionsAsync(OrganizationQuery query)

// ❌ Avoid
public class orgSvc
public async Task<IEnumerable<OrgSugResp>> GetSugs(OrgQuery q)
```

#### Files and Directories
- **PascalCase** for file names
- **Plural nouns** for directories containing multiple related items
- **Singular nouns** for directories containing single concept items

```
Controllers/        # Plural - contains multiple controllers
Entities/          # Plural - contains multiple entities
Common/           # Singular - contains shared utilities
```

### 2. Class Structure and Design Patterns

#### Primary Constructor Pattern
The codebase consistently uses C# 12 primary constructors:

```csharp
public class OrganizationController(OrganizationService organizationService) : ControllerBase
{
    // Fields are automatically created from constructor parameters
}

public class OrganizationService(IOrganizationRepository organizationRepository, IMapper mapper)
{
    // Dependencies injected via primary constructor
}
```

#### Repository Pattern
- **Base repository**: `CrudRepositoryBase<TEntity, TKey>` provides common CRUD operations
- **Specific repositories**: Inherit from base and add specialized methods
- **Interface segregation**: Each repository implements specific interface

```csharp
public class OrganizationRepository : CrudRepositoryBase<Organization, Guid>, IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext context) : base(context) { }
    
    // Specialized methods specific to Organization
    public async Task<Organization?> GetByNameAsync(string name) { ... }
}
```

#### Service Layer Pattern
- **Application services** coordinate between repositories and handle business logic
- **Dependency injection** via primary constructors
- **AutoMapper** for DTO mapping

### 3. Entity and DTO Conventions

#### Entity Design
- Implement `IEntity<TKey>` interface
- Use `required` keyword for mandatory properties
- Nullable reference types with `?` for optional properties
- **Soft delete** pattern with `DeletedAt` property
- **Audit fields**: `CreatedAt`, `UpdatedAt`, `DeletedAt`

```csharp
public class Organization : IEntity<Guid>
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? LogoUrl { get; set; }           // Optional
    public required OrganizationType OrganizationType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties initialized to empty collections
    public virtual ICollection<Experience> Experiences { get; set; } = [];
}
```

#### DTO Organization
- **Request/Response separation**: DTOs organized by operation type
- **Folder structure**: `DTOs/[EntityName]/[Requests|Responses]/`
- **Descriptive naming**: Include operation context in DTO names

```
DTOs/
└── Organization/
    ├── Requests/
    │   ├── CreateOrganizationRequestDto.cs
    │   ├── UpdateOrganizationRequestDto.cs
    │   └── OrganizationQuery.cs
    └── Responses/
        ├── OrganizationResponse.cs
        └── OrganizationSuggestionResponse.cs
```

### 4. API Controller Conventions

#### Controller Structure
- **Controller suffix**: All controllers end with "Controller"
- **Route attributes**: Use `[Route("api/[controller-name]")]` pattern
- **Authorization**: Apply `[Authorize]` at controller level
- **API documentation**: Include XML comments for Swagger

```csharp
[ApiController]
[Authorize]
[Route("api/organizations")]
public class OrganizationController(OrganizationService organizationService) : ControllerBase
{
    /// <summary>
    /// Searches organizations by name for autocomplete suggestions
    /// </summary>
    /// <param name="query">Query parameters including search term and pagination</param>
    /// <returns>List of organizations matching the search criteria</returns>
    [HttpGet("suggestions")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<OrganizationSuggestionResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetSuggestionsAsync([FromQuery] OrganizationQuery query)
    {
        var result = await organizationService.GetSuggestionsAsync(query);
        return Ok(result);
    }
}
```

### 5. Dependency Injection and Configuration

#### Service Registration Pattern
- **Extension methods** for organizing service registrations
- **Scoped lifetime** for most services
- **Configuration binding** using `IOptions<T>` pattern

```csharp
public static void ConfigureServices(this IServiceCollection services)
{
    // Repository registrations
    services.AddScoped<IOrganizationRepository, OrganizationRepository>();
    
    // Service registrations
    services.AddScoped<OrganizationService>();
    
    // Generic repository registration
    services.AddScoped(typeof(IGenericRepository<,>), typeof(CrudRepositoryBase<,>));
}
```

#### Configuration Classes
- **Settings classes** correspond to configuration sections
- **Required properties** use `required` keyword
- **Options pattern** with `IOptions<T>` for injection

### 6. Database and Entity Framework Conventions

#### Entity Configuration
- **Fluent API configuration** in separate configuration classes
- **Configuration classes** in `Data/Configurations/` folder
- **Applied in DbContext**: `modelBuilder.ApplyConfiguration(new EntityConfiguration())`

#### Migration and Seeding
- **Database migrations** in `Infrastructure/Migrations/`
- **Seed data** logic in `Data/Seeding/`
- **Automatic migration** on application startup (except in Testing environment)

### 7. Error Handling and Validation

#### Global Exception Handling
- **Custom middleware**: `ExceptionHandlingMiddleware`
- **Structured error responses** using `Error` type
- **HTTP status codes** aligned with REST conventions

#### Validation
- **FluentValidation** for request validation
- **Validators** registered automatically from assembly
- **Validation attributes** on DTO properties where appropriate

### 8. Asynchronous Programming

#### Async/Await Pattern
- **Async methods** end with "Async" suffix
- **Task return types** for all async operations
- **ConfigureAwait(false)** not used (ASP.NET Core context)

```csharp
public async Task<IEnumerable<Organization>> SearchByNameAsync(OrganizationQuery query)
{
    var queryable = _context.Organizations.AsNoTracking();
    // ... query building
    return await queryable.ToListAsync();
}
```

### 9. Code Quality Standards

#### General Principles
- **Single Responsibility**: Each class has one clear purpose
- **Dependency Inversion**: Depend on interfaces, not concrete types
- **Separation of Concerns**: Clear boundaries between layers
- **DRY (Don't Repeat Yourself)**: Reusable base classes and utilities

#### Nullable Reference Types
- **Enabled project-wide**: All projects use nullable reference types
- **Explicit nullability**: Use `?` for nullable properties
- **Required properties**: Use `required` keyword appropriately

#### Performance Considerations
- **AsNoTracking()** for read-only queries
- **Proper pagination** implementation
- **Efficient querying** with appropriate filtering and sorting

This document serves as a reference for maintaining consistency across the ServicePerfectCV codebase. All new code should adhere to these established patterns and conventions.