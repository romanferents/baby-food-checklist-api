# Baby Food Checklist API — Project Guidelines

## Architecture

This project follows **Clean Architecture** with strict dependency rules:

```
Domain ← Application ← Infrastructure ← API
```

- **Domain** (`BabyFoodChecklist.Domain`): Entities, enums, interfaces, events. Zero external dependencies.
- **Application** (`BabyFoodChecklist.Application`): CQRS via MediatR, FluentValidation, AutoMapper, DTOs. Depends only on Domain.
- **Infrastructure** (`BabyFoodChecklist.Infrastructure`): EF Core DbContext (PostgreSQL/Npgsql), repositories, seeders, interceptors. Depends on Application + Domain.
- **API** (`BabyFoodChecklist.API`): ASP.NET Core 8 controllers, middleware, OData. Depends on Application + Infrastructure for DI wiring only.

**Never** reference Infrastructure or API from Domain or Application.

## CQRS Pattern (MediatR)

Every feature follows: `Feature → Query/Command → Handler → Validator (optional)`

- Queries return DTOs (read-only, no side effects).
- Commands mutate state and return DTOs or `Unit`.
- Validators use FluentValidation and run via `ValidationBehavior<,>` pipeline.
- Handlers inject `IApplicationDbContext` or `IBaseRepository<T>`, never raw `DbContext`.

## Code Style

- C# 12, .NET 8, nullable reference types enabled.
- Records for DTOs (immutable, init-only).
- PascalCase for public members, `_camelCase` for private fields.
- `async/await` everywhere; always pass `CancellationToken`.
- Central package management via `Directory.Packages.props` — never specify versions in `.csproj`.

## Entities

- Inherit from `BaseEntity` (has `Id: Guid` + domain events), `BaseAuditableEntity` (adds audit timestamps), or `NamedEntity` (adds `NameUk`/`NameEn`).
- Bilingual: every user-facing name has both `NameUk` (Ukrainian) and `NameEn` (English).
- Default/seeded products have `IsDefault = true` and cannot be modified or deleted.

## Error Handling

- `NotFoundException` → 404, `ForbiddenException` → 403, `ValidationException` → 400 (with errors dict).
- `ExceptionHandlingMiddleware` maps all exceptions to RFC 7807 Problem Details.

## Testing

- NUnit 3 + FluentAssertions + Moq + AutoFixture + MockQueryable.
- Mock `IApplicationDbContext` with `BuildMockDbSet()` from MockQueryable.
- Test naming: `Method_ExpectedResult_WhenCondition`.

## Build & Run

```bash
docker-compose up postgres -d          # Start PostgreSQL
dotnet run --project src/BabyFoodChecklist.API  # Run API (seeds DB on first run)
dotnet test BabyFoodChecklist.sln      # Run all tests
```
