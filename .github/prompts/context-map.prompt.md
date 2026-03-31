---
description: "Generate a context map of all files affected by a planned change. Identifies files to modify, dependencies, related tests, and risk assessment. Use before implementing any significant feature or refactoring."
agent: "agent"
tools: [read, search]
argument-hint: "Describe the change, e.g. 'Add user authentication with JWT tokens'"
---
# Context Map — Impact Analysis

Before implementing any change, analyze the codebase and produce a context map.

## Instructions

1. **Search the codebase** for files related to the described change
2. **Trace the dependency chain** through all 4 layers:
   - Domain → entities, interfaces, enums affected
   - Application → features, DTOs, validators, mapping profiles affected  
   - Infrastructure → DbContext, configurations, repositories, seeders affected
   - API → controllers, middleware, Program.cs affected

3. **Find related tests** in `tests/BabyFoodChecklist.Tests/`

4. **Check for ripple effects**:
   - OData model changes (`ODataModelConfiguration.cs`)
   - Seed data changes (`DbSeeder.cs`)
   - DI registration changes (`DependencyInjection.cs` in Application + Infrastructure)
   - `CategoryHelper.cs` if categories are affected

## Output Format

```markdown
## Context Map: {Change Description}

### Files to Create
| File | Layer | Purpose |
|------|-------|---------|

### Files to Modify
| File | Layer | Changes Needed |
|------|-------|----------------|

### Dependencies (may need updates)
| File | Relationship |
|------|-------------|

### Test Files
| Test | Coverage |
|------|----------|

### Risk Assessment
- [ ] Breaking changes to API contracts
- [ ] Database schema changes (needs migration)
- [ ] OData model changes
- [ ] Seed data changes (re-seed required)
- [ ] DI registration changes
```
