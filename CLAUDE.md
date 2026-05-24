# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build src/Web.Api/Web.Api.csproj
dotnet build                                              # full solution (ModelProjectNet.sln)

# Run
dotnet watch run --project src/Web.Api/Web.Api.csproj    # API with hot reload

# Test
dotnet test                                              # all tests
dotnet test tests/UnitTests/UnitTests.csproj             # unit tests only
dotnet test --filter "FullyQualifiedName~ExampleRepositoryTests"   # single test class
dotnet test --filter "FullyQualifiedName~ExampleRepositoryTests.AddAsync_ShouldAddExample"  # single test

# EF Core migrations (run from repo root)
dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/Web.Api
dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api
```

Targets **.NET 10** (`net10.0`). The README still mentions ".NET Core 9" and a WorkerService — both are stale; the WorkerService project was removed.

### EF Core version constraint (important)

Keep **all** EF Core packages on the **9.x** line. `Pomelo.EntityFrameworkCore.MySql` (the MySQL provider) has no EF Core 10 release, so EF Core 9 is the ceiling even though the rest of the solution uses `Microsoft.Extensions.*` 10.x. Bumping a test/runtime EF package (e.g. `Microsoft.EntityFrameworkCore.InMemory`) to 10.x produces a runtime `MissingMethodException` because EF Core 9 assemblies are the ones actually loaded. `Infrastructure` also still pins `Microsoft.EntityFrameworkCore.Design` at 8.0.7 (design-time only; ideally aligned to 9.x for migration tooling).

## Architecture

Clean Architecture, four layers. Dependency rule: outer depends on inner; inner never references outer.

```
Domain         ← no external dependencies. Entities, Result/Error pattern,
                 repository interfaces (IRepositoryBase, IExampleRepository).
Application    ← depends on Domain. Application services (use cases), DTOs,
                 IExampleAppService, DependencyInjection.AddApplication().
Infrastructure ← depends on Domain. EF Core + MySQL (Pomelo), repository impls,
                 ViaCEP HTTP integration, DataContext, migrations, health checks,
                 DependencyInjection.AddInfrastructure(configuration).
Web.Api        ← depends on Application + Infrastructure. ASP.NET Core REST API,
                 Swagger/OpenAPI, API versioning (V1/V2), CORS, health checks,
                 correlation + global-exception middleware.
UnitTests      ← xUnit, Moq, FluentAssertions, EF Core InMemory provider.
```

`Program.cs` is the only composition root: it binds `AppSettings` from the `"Settings"` config section, calls `AddApplication()` then `AddInfrastructure(configuration)`, and wires the middleware pipeline (order matters): `CorrelationMiddleware` → `GlobalExceptionMiddleware` → controllers → CORS → auth → Swagger → HTTPS redirect → health checks → static files → OpenAPI.

### Result / Error pattern (`Domain/Common/`)

Functional error handling instead of throwing. `Result<T>` carries `Value` or `Error` and exposes `Match(onSuccess, onFailure)`; the non-generic `Result` carries only success/`Error`. `BaseController.HandleResult(...)` unwraps both, and `BaseController.HandleError` maps `Error.Code` (string codes like `"NOT_FOUND"`, `"VALIDATION_ERROR"`, `"BUSINESS_RULE_VIOLATION"`, `"DATABASE_UNAVAILABLE"`) to HTTP status codes. Controller actions should return `HandleResult(...)` rather than constructing `IActionResult` directly.

Convention nuance: query-style service methods return `Result<T>`, but some commands (e.g. `ExampleAppService.SyncCity`) return a bare `Task` and **throw** on failure — the throw is caught by `GlobalExceptionMiddleware` and turned into a 500. Such actions return a plain `NoContent()`/`Ok()`.

### API versioning & controllers

Controllers live under `Web.Api/Controllers/V1/` and `V2/` and inherit `BaseController`. Route template `api/v{version:apiVersion}/[controller]`; default version is v1, and the version is read from the URL segment or the `x-api-version` header. Each action is tagged with `[MapToApiVersion(...)]`. Swagger documentation strings are kept out of the controllers in `Web.Api/Markdown/`: shared `GlobalControllerMarkdown` lives in the `Web.Api.Markdown` namespace, while per-version operation text lives in `Web.Api.Markdown.V1` / `Web.Api.Markdown.V2` (`ExampleControllerMarkdown`). When adding an endpoint, add its `Summary`/`Description` to the matching versioned markdown class and reference it via `[SwaggerOperation(...)]` + `[SwaggerResponse(...)]`.

### Repository pattern

Interfaces in `Domain/Interfaces` (`IRepositoryBase`, `IExampleRepository`), implementations in `Infrastructure/Repositories` (`RepositoryBase`, `ExampleRepository`), registered scoped in `Infrastructure/DependencyInjection.cs`.

### External integrations

- **ViaCEP** — Brazilian postal-code/address API. `Infrastructure/ExternalService/ExampleService.cs` uses an injected `HttpClient` (registered via `AddHttpClient()`) whose `BaseAddress`/`Timeout` come from `AppSettings.Viacep`. HTTP calls go through `HttpClientExtensions.SendRequestAsync<TReq,TResp>`. The service swallows errors and returns an empty collection rather than throwing.
- **MySQL** — Pomelo provider with `ServerVersion.AutoDetect(connectionString)`; `DataContextFactory` (`IDesignTimeDbContextFactory`) enables design-time migrations by reading `../Web.Api/appsettings.json`.
- **Seq** — Serilog sink for structured logs, configured entirely from `appsettings.json` (`builder.Host.UseSerilog(... ReadFrom.Configuration ...)`).
- **Health checks** — MySQL health check tagged `ready`, registered in `AddInfrastructure`.

### Configuration

- Strongly-typed settings: `Infrastructure/Configuration/AppSettings.cs`, bound from the `"Settings"` section. (Note: `AppSettings` still contains a `QuartzJobs` list left over from the removed WorkerService — currently unused.)
- Connection string `DefaultConnection`, ViaCEP base URL/timeout, and `SwaggerBasicAuth` credentials live in `appsettings.json` / `appsettings.Development.json`.
- **Swagger is gated by HTTP Basic Auth** via `SwaggerAuthMiddleware` (checks the `SwaggerBasicAuth` section for any path under `/swagger`); it is registered at the end of `UseSwaggerSetup`.
