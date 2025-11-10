# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 9 Clean Architecture project in Portuguese (Brazilian), consisting of a REST API (Web.Api) and a background Worker Service. The architecture strictly enforces dependency rules: Domain has no dependencies, Application depends only on Domain, Infrastructure implements interfaces from Domain/Application, and presentation layers (Web.Api/WorkerService) consume Application and Infrastructure.

## Build, Run, and Test Commands

### Build
```bash
dotnet build ModelProjectNet.sln
```

### Run Web API (Development)
```bash
dotnet run --project src/Web.Api/Web.Api.csproj
```
API will be available at ports defined in [src/Web.Api/Properties/launchSettings.json](src/Web.Api/Properties/launchSettings.json)

### Run Worker Service
```bash
dotnet run --project src/WorkerService/WorkerService.csproj
```

### Run Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run specific test project
dotnet test tests/UnitTests/UnitTests.csproj
```

### Database Migrations
The project uses Entity Framework Core with MySQL (Pomelo provider).

```bash
# Add new migration (run from Infrastructure project directory)
cd src/Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Web.Api/Web.Api.csproj

# Update database
dotnet ef database update --startup-project ../Web.Api/Web.Api.csproj

# Remove last migration
dotnet ef migrations remove --startup-project ../Web.Api/Web.Api.csproj
```

**Important**: Migrations use [DataContextFactory.cs](src/Infrastructure/Context/DataContextFactory.cs) which reads connection string from [src/Web.Api/appsettings.json](src/Web.Api/appsettings.json). Ensure database connection is configured before running migrations.

## Architecture Patterns

### Result Pattern
The codebase uses the Result pattern (not exceptions) for handling operation outcomes:

- **[Domain/Common/Result.cs](src/Domain/Common/Result.cs)**: Generic `Result<T>` and non-generic `Result` types
- **[Domain/Common/Error.cs](src/Domain/Common/Error.cs)**: Standardized error codes (NOT_FOUND, VALIDATION_ERROR, BUSINESS_RULE_VIOLATION, UNAUTHORIZED, FORBIDDEN, CONFLICT, INTERNAL_ERROR)
- **[Web.Api/Controllers/BaseController.cs](src/Web.Api/Controllers/BaseController.cs)**: `HandleResult()` methods automatically convert Result to appropriate HTTP responses

When creating services or repositories, return `Result<T>` or `Result` instead of throwing exceptions for expected failures.

### Dependency Injection Registration
Each layer registers its dependencies via extension methods:

- **Application**: [Application/DependencyInjection.cs](src/Application/DependencyInjection.cs) - `AddApplication()`
- **Infrastructure**: [Infrastructure/DependencyInjection.cs](src/Infrastructure/DependencyInjection.cs) - `AddInfrastructure(IConfiguration)`
  - Registers repositories, external services, DbContext, and health checks
  - Database configuration uses MySQL with auto-detected server version

Registration follows this pattern in [Program.cs](src/Web.Api/Program.cs):
```csharp
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);
```

### Controller Pattern
All controllers inherit from [BaseController](src/Web.Api/Controllers/BaseController.cs) which provides:
- `HandleResult<T>(Result<T>)` - Converts Result to IActionResult with proper HTTP status
- `HandleError(Error)` - Maps error codes to HTTP status codes
- Standardized error response format with timestamp, path, method, traceId

Controllers are organized by API version ([V1/](src/Web.Api/Controllers/V1/), [V2/](src/Web.Api/Controllers/V2/)) and use `Asp.Versioning` with route pattern: `api/v{version:apiVersion}/[controller]`

### Entity Framework Configuration
- **[DataContext](src/Infrastructure/Context/DataContext.cs)**: Main DbContext, auto-discovers configurations via `ApplyConfigurationsFromAssembly`
- Entity configurations live in [Infrastructure/Configuration/](src/Infrastructure/Configuration/) implementing `IEntityTypeConfiguration<T>`
- Base entity class: [Domain/Entities/EntityBase.cs](src/Domain/Entities/EntityBase.cs)

### Background Jobs (Worker Service)
The Worker Service uses Quartz.NET for job scheduling:
- Job implementations in [src/WorkerService/Jobs/](src/WorkerService/Jobs/)
- Configuration via [QuartzConfiguration.cs](src/WorkerService/Configuration/QuartzConfiguration.cs)
- Jobs are registered from AppSettings configuration

### Middleware
Global middleware registered in [Program.cs](src/Web.Api/Program.cs):
- **[CorrelationMiddleware](src/Web.Api/Middleware/CorrelationMiddleware.cs)**: Adds correlation IDs to requests
- **[GlobalExceptionMiddleware](src/Web.Api/Middleware/GlobalExceptionMiddleware.cs)**: Centralized exception handling

### Logging
Both Web.Api and Worker Service use Serilog:
- Configuration in appsettings.json
- Console sink enabled by default
- Structured logging with correlation IDs

## Development Workflow

1. **Adding a new entity**:
   - Create entity in [src/Domain/Entities/](src/Domain/Entities/) inheriting from `EntityBase`
   - Create repository interface in [src/Domain/Interfaces/](src/Domain/Interfaces/) inheriting from `IRepositoryBase<T>`
   - Implement repository in [src/Infrastructure/Repositories/](src/Infrastructure/Repositories/) inheriting from `RepositoryBase<T>`
   - Create EF configuration in [src/Infrastructure/Configuration/](src/Infrastructure/Configuration/)
   - Register repository in [Infrastructure/DependencyInjection.cs](src/Infrastructure/DependencyInjection.cs)
   - Add migration

2. **Adding a new API endpoint**:
   - Create DTOs in [src/Application/DTO/](src/Application/DTO/)
   - Create service interface in [src/Application/Interfaces/](src/Application/Interfaces/)
   - Implement service in [src/Application/Services/](src/Application/Services/) returning `Result<T>`
   - Register service in [Application/DependencyInjection.cs](src/Application/DependencyInjection.cs)
   - Create controller in [src/Web.Api/Controllers/V{version}/](src/Web.Api/Controllers/) inheriting from `BaseController`
   - Use `HandleResult()` to convert service results to HTTP responses
   - Add Swagger documentation using `SwaggerOperation` and `SwaggerResponse` attributes

3. **Adding external service integration**:
   - Create service interface in [src/Infrastructure/ExternalService/Interface/](src/Infrastructure/ExternalService/Interface/)
   - Create DTOs in [src/Infrastructure/ExternalService/DTO/](src/Infrastructure/ExternalService/DTO/)
   - Implement service in [src/Infrastructure/ExternalService/](src/Infrastructure/ExternalService/)
   - Register in [Infrastructure/DependencyInjection.cs](src/Infrastructure/DependencyInjection.cs)
   - Add configuration to appsettings.json under "Settings"

4. **Writing tests**:
   - Unit tests organized by layer: [tests/UnitTests/Application/](tests/UnitTests/Application/), [tests/UnitTests/Infrastructure/](tests/UnitTests/Infrastructure/), [tests/UnitTests/Web.Api/](tests/UnitTests/Web.Api/)
   - Use xUnit, FluentAssertions, and Moq
   - Use EF InMemory provider for repository tests

## Project Structure Notes

- Code is primarily in Portuguese (comments, variable names, documentation)
- Swagger UI includes custom styling in [wwwroot/](src/Web.Api/wwwroot/)
- API documentation markdown files in [src/Web.Api/Markdown/](src/Web.Api/Markdown/)
- Health checks configured for MySQL database at `/health` endpoint
- CORS configuration in [Web.Api/Configurations/CorsConfiguration.cs](src/Web.Api/Configurations/CorsConfiguration.cs)
- Docker support via Dockerfiles in Web.Api and WorkerService projects
