---
name: api-health-audit
description: "Audit the API for architecture violations, missing tests, security issues, and code quality problems. Use when: reviewing code quality, checking architecture compliance, finding untested code, detecting missing validators, verifying Clean Architecture rules, running a pre-PR checklist, performing a health check on the codebase."
argument-hint: "Optional: focus area like 'test coverage' or 'architecture'"
---

# API Health Audit

Performs a comprehensive audit of the Baby Food Checklist API codebase across multiple dimensions: architecture compliance, test coverage gaps, security, and code quality.

## When to Use
- Before creating a pull request
- After a major feature addition
- Periodic codebase health review
- When onboarding to understand codebase quality

## Procedure

### Step 1: Architecture Compliance
Check for Clean Architecture violations:
- Search for forbidden `using` statements:
  - Domain files should NOT reference `Microsoft.EntityFrameworkCore`, `MediatR`, `AutoMapper`
  - Application files should NOT reference `BabyFoodChecklist.Infrastructure`
  - Controllers should NOT reference `BabyFoodChecklist.Domain.Entities` directly (use DTOs)
- Check `.csproj` `ProjectReference` elements match allowed dependencies
- Verify handlers inject interfaces, not concrete classes

### Step 2: Test Coverage Gaps
- List all command/query handlers in `Features/`
- List all test files in `tests/`
- Identify handlers without corresponding test files
- Check for missing test cases:
  - Every handler should test: happy path + NotFoundException + ForbiddenException (where applicable)
  - Every validator should test: valid input + each invalid field

### Step 3: Missing Validators
- Find all Commands (classes implementing `IRequest<>` in `Commands/` folders)
- Check each has a corresponding `*Validator.cs`
- Flag commands that accept user input but have no validation

### Step 4: Security Review
Refer to [security checklist](./references/security-checklist.md) for detailed checks.

### Step 5: Code Quality
- Check all async methods receive and pass `CancellationToken`
- Verify DTOs are `record` types with `init` properties
- Check entity string properties have `MaxLength` configured
- Verify bilingual consistency: every `NameUk` has a matching `NameEn`

## Output Format
```markdown
## Audit Report — {Date}

### Architecture ✅/❌
| Check | Status | Details |
|-------|--------|---------|

### Test Coverage ✅/❌
| Handler | Has Tests | Missing Cases |
|---------|-----------|---------------|

### Validators ✅/❌
| Command | Has Validator | 
|---------|---------------|

### Security ✅/❌
| Check | Status | Details |
|-------|--------|---------|

### Code Quality ✅/❌
| Check | Status | Details |
|-------|--------|---------|

### Summary
- **Score**: X/Y checks passed
- **Critical**: {issues requiring immediate attention}
- **Recommended**: {improvements to consider}
```
