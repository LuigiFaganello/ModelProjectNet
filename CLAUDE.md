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

# Local environment (API + MySQL)
docker compose up --build           # API on :8080, MySQL on :3306
```

Targets **.NET 10** (`net10.0`). The README still mentions ".NET Core 9" and a WorkerService — both are stale; the WorkerService project was removed.

### Build conventions & CI

- **Nullable reference types and .NET analyzers are enabled solution-wide** via `Directory.Build.props` (repo root); `.editorconfig` holds the formatting/style rules. The build is warning-clean — keep it that way (annotate nullability rather than suppressing).
- **CI**: `.github/workflows/ci.yml` runs restore → build (`Release`) → test with coverage on push/PR to `main`.

### Dependencies & EF Core version constraint (important)

Package versions are managed centrally via **Central Package Management**: `Directory.Packages.props` (repo root) holds every `<PackageVersion>`, and the `.csproj` files carry `<PackageReference>` **without** a `Version`. Add or bump dependencies in `Directory.Packages.props`, not in the project files.

Keep **all** EF Core packages on the **9.x** line (`Pomelo.EntityFrameworkCore.MySql`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.InMemory`). `Pomelo` has no EF Core 10 release, so EF Core 9 is the ceiling even though the rest of the solution uses `Microsoft.Extensions.*` 10.x. Bumping any EF package to 10.x produces a runtime `MissingMethodException` because EF Core 9 assemblies are the ones actually loaded.

## Commit conventions

This repo enforces a custom commit format (see `.claude/skills/conventional-commits/SKILL.md`; the `/commit` command gives a guided flow). It is **not** standard Conventional Commits:

- `<type>: (#<issue>) <issue_name> - <description>.` when a GitHub issue applies, else `<type>: <description>.` Multiline uses a `:`-terminated first line followed by `- `/`.`-delimited bullets.
- `<type>` is exactly one of `New feature`, `Fix issue`, `Other`.
- Present-tense imperative; single-line messages end with `.`. Keep commits atomic (one concern each).
- **Never add trailers** — no "Generated with Claude Code", author, or `Co-Authored-By` lines. This overrides the usual default of appending a co-author trailer.

## Claude Code tooling (`.claude/`)

This repo carries custom Claude Code tooling (adapted from the "Claude Code Best Practices" setup, trimmed to a backend focus).

**Custom slash commands** (`.claude/commands/`):

- `/commit` — guided conventional commit following `.gitmessage` (see *Commit conventions* above).
- `/issue <number>` — resolve a GitHub issue end-to-end via GitHub Flow.
- `/reviewpr <number>` — thorough PR review with structured feedback.
- `/test <scope>` — run and improve the test suite for a given scope.
- `/help-commands` — list all custom commands and their usage.

**Prerequisite:** `/issue` and `/reviewpr` require the GitHub CLI authenticated — `gh auth login` (verify with `gh repo view`).

**Specialized agents** (`.claude/agents/`) provide domain expertise; the slash commands orchestrate them, and you can also invoke any agent directly via the Agent tool. Grouped by role (this is a backend-focused subset — no frontend/fullstack agents):

*Core:*

- **general-solution-architect** — system architecture, technology-stack decisions, scalability and distributed-systems/microservices design, performance strategy. Read-only analysis (Read/Grep/Glob), no edits.
- **general-technical-writer** — create, review and improve technical docs: API documentation, READMEs, user/installation guides, troubleshooting.
- **general-pm** — product-management oversight: issue creation, prioritization and metadata, sprint/progress tracking, blocker identification, status updates and lifecycle management.

*Development:*

- **general-backend-developer** — design, implement and optimize backend APIs (REST), database schema design, performance tuning, error-handling strategies and monitoring.

*Quality Assurance:*

- **general-qa** — test planning and automation, edge-case identification, regression testing, and end-to-end validation strategies.
- **general-code-quality-debugger** — systematic code review, debugging, refactoring guidance, root-cause analysis and technical-debt reduction.
- **general-technical-project-lead** — principal-level technical leadership: performance optimization, security assessments and strategic/architectural review.

**Which command uses which agent** (from the command files):

- `/commit` → general-code-quality-debugger, general-technical-project-lead.
- `/issue` → general-backend-developer, general-qa (plus the built-in general-purpose agent).
- `/reviewpr` → general-code-quality-debugger, general-technical-project-lead, general-qa, general-solution-architect.
- `/test` → general-qa, general-code-quality-debugger, general-backend-developer.

`general-pm` and `general-technical-writer` aren't wired into a command — invoke them directly via the Agent tool when their expertise is needed.

**Skills** (`.claude/skills/`): `conventional-commits` (auto-enforces the commit format) and `drawio` (diagram generation).

**Workflow templates** referenced by the commands: `.gitmessage` (commit format), `.github/COMMIT_CONVENTION.md` (commit best practices), `.github/pull_request_template.md` (PR structure).

## Architecture

Clean Architecture, four layers. Dependency rule: outer depends on inner; inner never references outer.

```
Domain         ← no external dependencies. Entities, value objects, enums, Result/Error
                 pattern, repository interfaces (IRepositoryBase, IExampleRepository).
Application    ← depends on Domain only. Use-case services, DTOs, the inbound interface
                 IExampleAppService AND the outbound PORTS for external systems
                 (IExampleService + its AddressDto contract), DependencyInjection.AddApplication().
Infrastructure ← depends on Domain + Application. Implements Domain repository interfaces
                 AND Application ports (the ViaCEP adapter). EF Core + MySQL (Pomelo),
                 DataContext, migrations, health checks, DependencyInjection.AddInfrastructure(configuration).
Web.Api        ← depends on Application + Infrastructure. ASP.NET Core REST API,
                 Swagger/OpenAPI, API versioning (V1/V2), CORS, health checks,
                 correlation + global-exception middleware.
UnitTests      ← xUnit, Moq, FluentAssertions, EF Core InMemory provider.
```

**Ports & Adapters (important):** abstractions for anything Infrastructure provides live in the
**inner** layers, never in Infrastructure. Repository interfaces live in `Domain/Interfaces`;
the external-service port `IExampleService` (and its `AddressDto` contract) lives in
`Application/Interfaces` + `Application/DTO`. Infrastructure only *implements* them. **Application
must never reference Infrastructure** — adding that project reference reintroduces the dependency-rule
violation this template was built to avoid.

`Program.cs` is the only composition root: it binds `AppSettings` from the `"Settings"` config section,
calls `AddApplication()` then `AddInfrastructure(configuration)`, and wires the middleware pipeline
(order matters): `CorrelationMiddleware` → `GlobalExceptionMiddleware` → HTTPS redirect → static files
→ CORS → authorization → Swagger → health checks → controllers → OpenAPI.

### Result / Error pattern (`Domain/Common/`)

Functional error handling instead of throwing. `Result<T>` carries `Value` or `Error` and exposes `Match(onSuccess, onFailure)`; the non-generic `Result` carries only success/`Error`. `BaseController.HandleResult(...)` unwraps both, and `BaseController.HandleError` maps `Error.Code` (string codes like `"NOT_FOUND"`, `"VALIDATION_ERROR"`, `"BUSINESS_RULE_VIOLATION"`, `"DATABASE_UNAVAILABLE"`) to HTTP status codes. Controller actions should return `HandleResult(...)` rather than constructing `IActionResult` directly.

Convention nuance: query-style service methods return `Result<T>`, but some commands (e.g. `ExampleAppService.SyncCity`) return a bare `Task` and **throw** on failure — the throw is caught by `GlobalExceptionMiddleware` and turned into a 500. Such actions return a plain `NoContent()`/`Ok()`.

### API versioning & controllers

Controllers live under `Web.Api/Controllers/V1/` and `V2/` and inherit `BaseController`. Route template `api/v{version:apiVersion}/[controller]`; default version is v1, and the version is read from the URL segment or the `x-api-version` header. Each action is tagged with `[MapToApiVersion(...)]`. Swagger documentation strings are kept out of the controllers in `Web.Api/Markdown/`: shared `GlobalControllerMarkdown` lives in the `Web.Api.Markdown` namespace, while per-version operation text lives in `Web.Api.Markdown.V1` / `Web.Api.Markdown.V2` (`ExampleControllerMarkdown`). When adding an endpoint, add its `Summary`/`Description` to the matching versioned markdown class and reference it via `[SwaggerOperation(...)]` + `[SwaggerResponse(...)]`.

### Repository pattern

Interfaces in `Domain/Interfaces` (`IRepositoryBase`, `IExampleRepository`), implementations in `Infrastructure/Repositories` (`RepositoryBase`, `ExampleRepository`), registered scoped in `Infrastructure/DependencyInjection.cs`.

### External integrations

- **ViaCEP** — Brazilian postal-code/address API. The port `Application/Interfaces/IExampleService.cs` is implemented by the adapter `Infrastructure/ExternalService/ExampleService.cs`, registered as a **typed `HttpClient`** (`AddHttpClient<IExampleService, ExampleService>(...)` in `Infrastructure/DependencyInjection.cs`) whose `BaseAddress`/`Timeout` come from `Settings:Viacep`. HTTP calls go through `HttpClientExtensions.SendRequestAsync<TReq,TResp>` (serialization is **System.Text.Json**; the project does **not** use Newtonsoft.Json). The adapter translates the provider DTO (`ExampleServiceDTO`, Portuguese ViaCEP field names) into the neutral `AddressDto`, swallows errors, and returns an empty collection rather than throwing. URL segments are encoded with `Uri.EscapeDataString`.
- **MySQL** — Pomelo provider with `ServerVersion.AutoDetect(connectionString)`; `DataContextFactory` (`IDesignTimeDbContextFactory`) enables design-time migrations by reading `../Web.Api/appsettings.json`.
- **Serilog** — structured logging configured from `appsettings.json` (`builder.Host.UseSerilog(... ReadFrom.Configuration ...)`); the Console sink is the only configured sink.
- **Health checks** — MySQL health check tagged `ready`, registered in `AddInfrastructure`.

### Observability

`CorrelationMiddleware` (first in the pipeline) reads an incoming `X-Correlation-Id` header or generates a GUID, stores it in `HttpContext.Items["CorrelationId"]`, echoes it back on the response header, and pushes it into Serilog's `LogContext` — so **every log line during the request carries `CorrelationId`** (the Console output template surfaces it). `GlobalExceptionMiddleware` includes the same `correlationId` in error response bodies. When adding logging, rely on the ambient `LogContext` property rather than threading the id manually.

### Configuration

- Strongly-typed settings: `Infrastructure/Configuration/AppSettings.cs`, bound from the `"Settings"` section (currently holds the ViaCEP base URL/timeout).
- Connection string `DefaultConnection`, ViaCEP base URL/timeout, and `SwaggerBasicAuth` credentials live in `appsettings.json` / `appsettings.Development.json`. The committed values are **local-dev defaults**; override secrets via user-secrets or environment variables in real deployments (the connection string is also overridden by `ConnectionStrings__DefaultConnection` in `docker-compose.yml`).
- **CORS** is configurable: `Cors:AllowedOrigins` (string array). When non-empty the policy restricts to those origins (with credentials); when empty it falls back to a permissive `AllowAnyOrigin` policy — acceptable for dev only. Configured in `Web.Api/Configurations/CorsConfiguration.cs`.
- **Swagger is gated by HTTP Basic Auth** via `SwaggerAuthMiddleware` (checks the `SwaggerBasicAuth` section for any path under `/swagger`); it is registered at the end of `UseSwaggerSetup`.

## Project structure (folder-by-folder)

This is a **template** for new services. Every folder is a deliberate "slot"; when adding code,
put each artifact in the folder that matches its role so the architecture stays consistent.
Folders marked **(slot)** are intentionally empty placeholders (a `.gitkeep` keeps them in git) —
they document where that kind of code goes; fill them as features require.

### `src/Domain/` — `Domain.csproj` (no external dependencies)

The innermost layer: enterprise rules and types. References nothing outside the BCL.

- **`Common/`** — cross-cutting domain primitives. `Error.cs` (record of `Code`/`Message`/optional `Details`, with factory helpers like `NotFound`, `Validation`, `BusinessRule`) and `Result.cs` (`Result` and `Result<T>` for functional success/failure without throwing).
- **`Entities/`** — business entities. `EntityBase.cs` (base with `Id` GUID, `CreatedDate`, `UpdatedDate`, all private-set) and `Example.cs` (sample entity with private setters + constructor). New entities inherit `EntityBase`.
- **`Enums/`** — **(slot)** domain enumerations shared by entities/value objects.
- **`ValueObjects/`** — **(slot)** immutable types without identity (e.g. `ZipCode`, `Email`, `Money`), validated in their constructor. Prefer these over primitive strings for domain concepts.
- **`Exceptions/`** — domain exceptions. `DomainException.cs` (abstract base with `ErrorCode`, plus `BusinessRuleViolationException`, `EntityNotFoundException`). Thrown for invariant violations; mapped to HTTP codes by `GlobalExceptionMiddleware`.
- **`Interfaces/`** — abstractions the outer layers implement. `IRepositoryBase.cs` (generic repo contract) and `IExampleRepository.cs` (`IExampleRepository : IRepositoryBase<Example>`). Repository ports live **here**, not in Infrastructure.

### `src/Application/` — `Application.csproj` (references **Domain only**)

Use cases / orchestration. Depends only on Domain. Holds the inbound interfaces (what the API calls)
and the outbound **ports** (what the app needs from the outside world).

- **`Common/`** — application-level primitives. `ApplicationException.cs` (`ApplicationException` + `ValidationException` carrying a field-error dictionary).
- **`DTO/`** — data-transfer objects. `ExampleAppServiceDto.cs` (shape returned to the API) and `AddressDto.cs` (neutral contract returned by the `IExampleService` port, keeping Application independent of any provider's wire format).
- **`Interfaces/`** — **inbound** use-case interfaces (`IExampleAppService.cs`) **and outbound ports** for external systems (`IExampleService.cs`). Both share the `Application.Interfaces` namespace.
- **`Mappings/`** — **(slot)** Entity↔DTO mapping profiles/extensions. Today mapping is done by hand inside the AppServices; AutoMapper/Mapster profiles would live here (no mapping dependency is wired by default).
- **`Services/`** — use-case implementations (`ExampleAppService.cs`). Query methods return `Result<T>`; command methods may return a bare `Task` and throw on failure (see Result/Error nuance above).
- **`Validators/`** — **(slot)** input validation for DTOs/commands (FluentValidation or guard clauses; no dependency wired by default).
- **`DependencyInjection.cs`** — `AddApplication()` registers the AppServices (scoped).

### `src/Infrastructure/` — `Infrastructure.csproj` (references **Domain + Application**)

Adapters for everything external. Implements Domain repository interfaces **and** Application ports.

- **`Configuration/`** — `AppSettings.cs` (strongly-typed `Settings` section, e.g. `Viacep`) and `ExampleEntityConfiguration.cs` (EF Core `IEntityTypeConfiguration<Example>`: table/column mapping, indexes). Add one `*EntityConfiguration` per entity; they're auto-discovered via `ApplyConfigurationsFromAssembly`.
- **`Context/`** — `DataContext.cs` (the `DbContext`; `DbSet`s and `SaveChanges` override that stamps `CreatedDate`/`UpdatedDate`) and `DataContextFactory.cs` (`IDesignTimeDbContextFactory` so `dotnet ef` works at design time by reading `../Web.Api/appsettings.json`).
- **`Extensions/`** — `HttpClientExtensions.cs` (`SendRequestAsync<TReq,TResp>` helper using System.Text.Json).
- **`ExternalService/`** — adapters that implement Application ports. `ExampleService.cs` (ViaCEP adapter) and **`DTO/`** with `ExampleServiceDTO.cs` (provider-specific wire model with ViaCEP's Portuguese field names; never leaves Infrastructure — it's translated to `AddressDto`).
- **`Migrations/`** — EF Core migrations (generated; don't hand-edit). Create with the `dotnet ef migrations add` command above.
- **`Repositories/`** — `RepositoryBase.cs` (generic implementation of `IRepositoryBase<T>`) and `ExampleRepository.cs` (`: RepositoryBase<Example>, IExampleRepository`).
- **`DependencyInjection.cs`** — `AddInfrastructure(configuration)`: registers repositories (scoped), the typed `HttpClient` adapter, the `DataContext` (Pomelo/MySQL), and the MySQL health check.

### `src/Web.Api/` — `Web.Api.csproj` (references **Application + Infrastructure**, `Microsoft.NET.Sdk.Web`)

Presentation layer: REST controllers, middleware, cross-cutting host configuration.

- **`Configurations/`** — host wiring split into extension methods: `CorsConfiguration.cs` (configurable origins), `HealthcheckConfiguration.cs` (`/healthcheck`, `/healthcheck/db`, `/healthcheck/app`), and **`Swagger/`** (`SwaggerConfiguration.cs` — API versioning + Swagger gen; `ConfigureSwaggerOptions.cs` — per-version Swagger docs).
- **`Controllers/`** — `BaseController.cs` (`HandleResult`/`HandleError` translate `Result`/`Error` into `IActionResult`); **`V1/`** (`ExampleController.cs`) and **`V2/`** **(slot)** for v2 controllers. Controllers inherit `BaseController` and return `HandleResult(...)`.
- **`Markdown/`** — Swagger text kept out of controllers: `GlobalControllerMarkdown.cs` (shared status-code descriptions) and per-version **`V1/`** (`ExampleControllerMarkdown.cs`) / **`V2/`** **(slot)**.
- **`Middleware/`** — `CorrelationMiddleware.cs` (correlation id → `LogContext`), `GlobalExceptionMiddleware.cs` (exception → JSON error + status), `SwaggerAuthMiddleware.cs` (Basic Auth gate for `/swagger`).
- **`Properties/`** — `launchSettings.json` (local debug profiles). **`wwwroot/`** — static assets.
- **`Program.cs`** — composition root + pipeline (see Architecture). **`appsettings*.json`** — config. **`Dockerfile`** — container build (context is repo root, see `docker-compose.yml`).

### `tests/UnitTests/` — `UnitTests.csproj` (references Application + Infrastructure)

xUnit + Moq + FluentAssertions + EF Core InMemory. Test folders mirror `src/` so tests are easy to locate.

- **`Application/`** — `ExampleAppServiceTests.cs` (use-case tests with mocked repo + port).
- **`Infrastructure/`** — `ExternalService/ExampleServiceTests.cs` (adapter, mocked `HttpMessageHandler`) and `Repositories/ExampleRepositoryTests.cs` (InMemory provider; **each test uses a unique DB name** to stay isolated).
- **`Domain/`** — **(slot)** entity / value-object / `Result` tests.
- **`Web.Api/`** — **(slot)** controller / `BaseController` / middleware tests.
- **`GlobalUsings.cs`** — global `using Xunit;`.

### Repo root

`ModelProjectNet.sln`, `Directory.Build.props` (nullable + analyzers solution-wide), `Directory.Packages.props` (Central Package Management — all `<PackageVersion>`s), `.editorconfig`, `.gitmessage` (commit template), `docker-compose.yml` (API + MySQL), `.github/workflows/ci.yml` (CI), `docs/` (architecture/C4 diagrams), `README.md`.

## Creating a new feature/project from this template

This repo is meant to be cloned/generated as the starting point for new services (including
AI-assisted scaffolding). To add a new aggregate — say `Product` with CRUD — touch the layers
**inside-out**, keeping the dependency rule intact:

1. **Domain** — add `Entities/Product.cs` (inherit `EntityBase`, private setters + constructor); add any `ValueObjects/`/`Enums/`; add the port `Interfaces/IProductRepository.cs : IRepositoryBase<Product>`.
2. **Infrastructure** — add `Repositories/ProductRepository.cs` (`: RepositoryBase<Product>, IProductRepository`); add `Configuration/ProductEntityConfiguration.cs`; register `IProductRepository` in `AddInfrastructure`; then `dotnet ef migrations add AddProduct` and `database update`.
3. **Application** — add `DTO/ProductDto.cs`; `Interfaces/IProductAppService.cs`; `Services/ProductAppService.cs` (return `Result<T>` for queries; `throw` for commands); register it in `AddApplication()`; add `Validators/`/`Mappings/` as needed.
4. **Web.Api** — add `Controllers/V1/ProductController.cs : BaseController`, returning `HandleResult(...)`; add operation text to `Markdown/V1/ProductControllerMarkdown.cs` and reference it via `[SwaggerOperation]`/`[SwaggerResponse]`.
5. **External system?** — define the **port** in `Application/Interfaces` (+ a neutral DTO in `Application/DTO`), implement the **adapter** in `Infrastructure/ExternalService`, and register it as a typed `HttpClient` in `AddInfrastructure`. Never reference Infrastructure from Application.
6. **Tests** — mirror the structure under `tests/UnitTests/{Domain,Application,Infrastructure,Web.Api}/`.

**Guardrails for any change in this repo:** keep the build warning-clean (nullable annotations, no suppressions); add packages only in `Directory.Packages.props`; keep EF Core packages on the 9.x line; respect the `Result`/`Error` + `HandleResult` convention; never add a project reference from Application to Infrastructure; follow the commit format in `.gitmessage` with no trailers.
