---
name: cqrs-scaffolder
description: "Scaffold a complete CQRS feature across all Clean Architecture layers. Use when: adding a new feature, creating a new command or query, building a new endpoint, scaffolding CRUD operations, adding a new entity with full stack support. Generates handler, validator, DTO, mapping profile, controller, and test files."
argument-hint: "Describe the feature to scaffold"
---

# CQRS Feature Scaffolder

Automates the creation of a complete CQRS feature across all 4 layers of the Clean Architecture, following project conventions exactly.

## When to Use
- Adding a new query (e.g., "Get products by category")
- Adding a new command (e.g., "Batch update entries")
- Creating a full CRUD for a new entity
- Extending existing features with new operations

## Procedure

### Step 1: Analyze the Request
Determine from the user's description:
- **Type**: Query (read) or Command (write)
- **Entity**: Which domain entity is involved
- **Input**: What parameters are needed
- **Output**: What DTO to return
- **Validation**: What rules apply to inputs

### Step 2: Read Reference Patterns
Load these files to match current conventions exactly:
- [Query pattern](./references/query-pattern.md) — for read operations
- [Command pattern](./references/command-pattern.md) — for write operations
- [Test pattern](./references/test-pattern.md) — for unit tests

### Step 3: Generate Files

For a **Query**, create:
1. `src/BabyFoodChecklist.Application/Features/{Feature}/Queries/{Name}/{Name}Query.cs`
2. `src/BabyFoodChecklist.Application/Features/{Feature}/Queries/{Name}/{Name}QueryHandler.cs`
3. DTO record in `src/BabyFoodChecklist.Application/DTOs/` (if new)
4. Mapping profile in `src/BabyFoodChecklist.Application/MappingProfiles/` (if new)
5. Controller endpoint in `src/BabyFoodChecklist.API/Controllers/`
6. Unit test in `tests/BabyFoodChecklist.Tests/Features/`

For a **Command**, also create:
7. `src/BabyFoodChecklist.Application/Features/{Feature}/Commands/{Name}/{Name}CommandValidator.cs`

### Step 4: Wire Up
- Verify AutoMapper profile is registered (assembly scanning handles this)
- Verify FluentValidation validator is registered (assembly scanning handles this)
- Add controller action with correct HTTP method and route

### Step 5: Verify
- Run `dotnet build BabyFoodChecklist.sln` to check compilation
- Run `dotnet test BabyFoodChecklist.sln` to verify tests pass

## Output
Provide a summary table of all files created and their purposes.
