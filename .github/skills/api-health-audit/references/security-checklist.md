# Security Checklist

## Input Validation
- [ ] All commands with user input have FluentValidation validators
- [ ] String properties have `MaximumLength()` rules matching EF config
- [ ] Enum properties use `.IsInEnum()` validation
- [ ] Guid properties use `.NotEmpty()` (not just `.NotNull()`)
- [ ] `ReactionNote` max 500, `Notes` max 1000, names max 200

## Entity Framework / SQL Injection
- [ ] All queries use LINQ (parameterized) — no raw SQL (`FromSqlRaw`)
- [ ] No string concatenation in query filters
- [ ] OData `$filter` uses EF Core provider (auto-parameterized)

## Error Handling
- [ ] `ExceptionHandlingMiddleware` catches all exception types
- [ ] Stack traces NOT exposed in production error responses
- [ ] Validation errors return field-level details (not raw exception)
- [ ] `NotFoundException` does not leak internal entity details beyond name + ID

## API Security
- [ ] Connection string not hardcoded in committed `appsettings.json` (use env vars in prod)
- [ ] `AllowedHosts` configured (not just `*` in production)
- [ ] CORS policy configured if front-end is cross-origin
- [ ] Health check endpoint does not expose sensitive database info

## Business Logic
- [ ] `IsDefault` products cannot be modified or deleted
- [ ] `ForbiddenException` thrown (not silently ignored) for default product mutations
- [ ] Upsert operation correctly checks product existence before creating entry

## OData
- [ ] `$top` has a max limit (currently 100) — prevents unbounded queries
- [ ] `$expand` depth limited to prevent N+1 explosion
- [ ] OData routes are read-only (GET only) — mutations go through REST controllers
