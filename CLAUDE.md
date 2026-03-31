# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build src/Web.Api/Web.Api.csproj
dotnet build  # full solution

# Run
dotnet watch run --project src/Web.Api/Web.Api.csproj   # API with hot reload
dotnet run --project src/WorkerService/WorkerService.csproj

# Test
dotnet test                                               # all tests
dotnet test tests/UnitTests/UnitTests.csproj             # unit tests only
dotnet test --filter "FullyQualifiedName~MyTestClass"    # single test class

# EF Core migrations (run from repo root)
dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/Web.Api
dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api
```

## Architecture

Clean Architecture with four layers. Dependency rule: outer layers depend on inner; inner layers never reference outer.

```
Domain       ← no external dependencies; defines entities, Result<T>/Error pattern,
               repository interfaces (IRepositoryBase, IExampleRepository)
Application  ← depends on Domain; use cases, application services, DTOs, DI registration
Infrastructure ← depends on Domain; EF Core + MySQL (Pomelo), repository implementations,
                 external HTTP service integrations, DataContext, migrations
Web.Api      ← depends on Application + Infrastructure; ASP.NET Core REST API,
               Swagger/OpenAPI, API versioning (v1/v2), CORS, health checks,
               correlation ID middleware, global exception middleware
WorkerService ← depends on Application + Infrastructure; background jobs via Quartz.NET
UnitTests    ← tests Application, Infrastructure, Web.Api layers using xUnit, Moq,
               FluentAssertions, EF Core InMemory provider
```

### Key patterns

- **Result<T>/Error** (`Domain/Common/`) — functional error handling; services return `Result<T>` instead of throwing
- **Repository pattern** — interfaces in Domain, implementations in Infrastructure
- **DependencyInjection.cs** — each layer (Application, Infrastructure) has its own DI registration file wired into `Program.cs`
- **API versioning** — controllers under `Web.Api/Controllers/V1/` and `V2/`; `BaseController.cs` holds shared logic

### External integrations

- **MySQL** via Pomelo EF Core provider; `DataContextFactory` enables design-time migrations
- **Seq** — structured log aggregation sink (Serilog)

### Configuration

- Connection strings and external API URLs live in `appsettings.json` / `appsettings.Development.json`
- Swagger Basic Auth and Quartz cron schedules are also in `appsettings.json`
- `Infrastructure/Configuration/AppSettings.cs` is the strongly-typed settings class
