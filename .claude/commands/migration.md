---
description: Create and apply an EF Core migration following the project conventions
argument-hint: <MigrationName> [--no-update]
---

# Claude Code User Command: Migration

This command creates a new EF Core migration (and, by default, applies it to the local database) for the change described in $ARGUMENTS, respecting this repo's Clean Architecture layout and EF Core version constraints.

## Usage

To create and apply a migration:

```
/migration AddProduct
```

To create the migration **without** updating the database (generate only):

```
/migration AddProduct --no-update
```

The first token of $ARGUMENTS is the **migration name** (PascalCase, descriptive of the schema change, e.g. `AddProduct`, `AddEmailIndexToExample`). The optional `--no-update` flag skips `dotnet ef database update`.

## Project facts (read before running)

- **EF tooling targets the Infrastructure project, started from Web.Api.** Migrations live in `src/Infrastructure/Migrations`.
- **EF Core packages are pinned to the 9.x line** (`Pomelo.EntityFrameworkCore.MySql` has no EF Core 10 release). Never bump an EF package to 10.x — it produces a runtime `MissingMethodException`. Versions are managed centrally in `Directory.Packages.props`.
- **Design-time factory:** `Infrastructure/Context/DataContextFactory.cs` (`IDesignTimeDbContextFactory`) reads `../Web.Api/appsettings.json` so `dotnet ef` works without a running host. The local `DefaultConnection` must be valid (MySQL reachable) for `database update`.
- **Entity configurations are auto-discovered** via `ApplyConfigurationsFromAssembly`. A new entity needs its `*EntityConfiguration : IEntityTypeConfiguration<T>` in `Infrastructure/Configuration/` **and** a `DbSet<T>` on `DataContext` before the migration captures the table.
- **`dotnet ef`** must be installed (`dotnet tool install --global dotnet-ef`, kept on the 9.x line). Run all commands **from the repo root**.

## What This Command Does

1. Parses $ARGUMENTS into the migration name and the optional `--no-update` flag; validates the name is PascalCase and non-empty (ask the user if missing).
2. Confirms `dotnet ef` is available and on a 9.x version; offers to install/pin it if not.
3. Verifies the model is ready for the migration:
   - If this is a **new entity**, checks there's a `DbSet<T>` on `DataContext` and a matching `*EntityConfiguration` in `Infrastructure/Configuration/`. Flags anything missing before generating.
   - Builds the solution (`dotnet build`) so the migration is generated against current code, warning-clean.
4. Generates the migration:
   ```bash
   dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/Web.Api
   ```
5. Reviews the generated `Up`/`Down` in `src/Infrastructure/Migrations/` — confirms it matches the intended change, columns/indexes/constraints look right, and `Down` cleanly reverses `Up`. Surfaces anything surprising (dropped columns, data-loss operations) before applying.
6. Unless `--no-update` was passed, applies it:
   ```bash
   dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api
   ```
7. Reports the created files and the resulting database state. Reminds the user that migration files are generated — don't hand-edit unless intentional.

## Guardrails

- **Never** change EF Core package versions to fix a tooling error — diagnose the actual cause (missing `DbSet`, unreachable MySQL, stale build) instead.
- Don't add packages or versions in `.csproj`; central versions live in `Directory.Packages.props`.
- If `database update` fails because MySQL isn't reachable, suggest `docker compose up` (API + MySQL) and retry — don't silently skip the apply step.
- Keep the change atomic; one schema concern per migration.

## Agents Used

- **general-backend-developer** — primary agent for schema/migration design and EF Core specifics.
- **general-code-quality-debugger** — reviews the generated migration and diagnoses tooling/runtime failures.
- **general-solution-architect** — consulted when the schema change touches the domain model or crosses architectural boundaries.

## After the migration

Offer to commit with `/commit` following the repo's commit format, e.g.:

```
New feature: add Product table migration.
```

(Use `Other:` for non-feature schema tweaks; never add trailers.)
