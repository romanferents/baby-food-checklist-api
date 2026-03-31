---
description: "Use when creating new classes, adding references, or modifying project dependencies. Enforces Clean Architecture layer boundaries: Domain has zero dependencies, Application depends only on Domain, Infrastructure depends on Application+Domain, API depends on Application+Infrastructure. Prevents layer violations, circular references, and leaky abstractions."
applyTo: ["src/**/*.cs", "src/**/*.csproj"]
---
# Clean Architecture Layer Rules

## Dependency Direction (Inner → Outer)
```
Domain → Application → Infrastructure → API
```

## Layer Rules

### Domain (`BabyFoodChecklist.Domain`)
- ZERO project references — this layer owns entities, enums, interfaces, and events
- Entities inherit from `BaseEntity`, `BaseAuditableEntity`, or `NamedEntity`
- Interfaces like `IApplicationDbContext` and `IBaseRepository<T>` are defined here but implemented in Infrastructure
- NEVER reference `Microsoft.EntityFrameworkCore`, `MediatR`, `AutoMapper`, or any outer-layer package
- Domain events extend `BaseEvent`

### Application (`BabyFoodChecklist.Application`)
- References ONLY `BabyFoodChecklist.Domain`
- Contains: CQRS features (MediatR), validators (FluentValidation), DTOs (records), mapping profiles (AutoMapper), pipeline behaviors
- Handlers inject `IApplicationDbContext` or `IBaseRepository<T>` — never `ApplicationDbContext` directly
- DTOs are C# records with `init` properties
- NEVER reference `BabyFoodChecklist.Infrastructure` or `BabyFoodChecklist.API`

### Infrastructure (`BabyFoodChecklist.Infrastructure`)
- References `BabyFoodChecklist.Domain` and `BabyFoodChecklist.Application`
- Contains: `ApplicationDbContext`, entity configurations, repositories, seeders, interceptors, OData model config
- Implements interfaces from Domain (`IApplicationDbContext`, `IBaseRepository<T>`)
- NEVER reference `BabyFoodChecklist.API`

### API (`BabyFoodChecklist.API`)
- References `BabyFoodChecklist.Application` and `BabyFoodChecklist.Infrastructure`
- Controllers inject `ISender` (MediatR) — never repositories or DbContext
- Only calls Application layer via MediatR `Send()`
- Infrastructure is referenced solely for DI registration in `Program.cs`

## Common Violations to Avoid
- Injecting `ApplicationDbContext` in a controller → use `ISender` + MediatR query/command
- Adding EF Core packages to Application layer → keep DB concerns in Infrastructure
- Putting business logic in controllers → move to command/query handlers
- Domain referencing Application types → move shared types to Domain
