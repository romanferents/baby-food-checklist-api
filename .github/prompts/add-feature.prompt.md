---
description: "Scaffold a complete CQRS feature end-to-end: command/query, handler, validator, DTO, mapping profile, controller endpoint, and unit test. Generates all files following project conventions."
agent: "agent"
model: ["Claude Sonnet 4 (copilot)", "GPT-4.1 (copilot)"]
tools: [read, edit, search, create_file]
argument-hint: "Describe the feature, e.g. 'GetProductsByCategory query that returns a list of products filtered by category enum'"
---
# Add CQRS Feature

Generate a complete CQRS feature for the Baby Food Checklist API.

## Instructions

1. **Gather context** — Read these files to understand current patterns:
   - [Product feature](src/BabyFoodChecklist.Application/Features/Products/) for command/query examples
   - [ProductDto](src/BabyFoodChecklist.Application/DTOs/ProductDto.cs) for DTO record pattern
   - [ProductMappingProfile](src/BabyFoodChecklist.Application/MappingProfiles/ProductMappingProfile.cs)
   - [ProductsController](src/BabyFoodChecklist.API/Controllers/ProductsController.cs)
   - [Existing tests](tests/BabyFoodChecklist.Tests/Features/) for test patterns

2. **Create files** following the folder structure:
   ```
   Features/{Feature}/Commands|Queries/{Action}/
   ├── {Action}Command|Query.cs
   ├── {Action}CommandHandler|QueryHandler.cs
   └── {Action}CommandValidator.cs (if command with input)
   ```

3. **DTO** — Use `record` with `init` properties. Add to `DTOs/` if new.

4. **Mapping Profile** — Create or extend in `MappingProfiles/`. Use `CategoryHelper` for bilingual category names.

5. **Controller endpoint** — Add to the appropriate controller. Inject `ISender`. Return proper HTTP status:
   - GET → `Ok(result)`
   - POST → `CreatedAtAction(..., result)` 
   - PUT → `Ok(result)`
   - DELETE → `NoContent()`

6. **Unit test** — Create in `tests/BabyFoodChecklist.Tests/Features/`. Follow naming: `Method_ExpectedResult_WhenCondition`. Use `Mock<IApplicationDbContext>` + `BuildMockDbSet()`.

7. **Bilingual** — All user-facing name fields must have both `NameUk` and `NameEn`.

## Output
Confirm all created files and show the controller endpoint signature.
