---
description: "Use when creating new CQRS features, commands, queries, handlers, or validators with MediatR. Covers the full feature pattern: folder structure, naming conventions, handler dependencies, FluentValidation rules, and DTO mapping. Applies to Products, Entries, Statistics, Categories features."
applyTo: "src/BabyFoodChecklist.Application/Features/**/*.cs"
---
# CQRS Feature Pattern

## Folder Structure
```
Features/
└── {FeatureName}/
    ├── Commands/
    │   └── {Action}{Entity}/
    │       ├── {Action}{Entity}Command.cs
    │       ├── {Action}{Entity}CommandHandler.cs
    │       └── {Action}{Entity}CommandValidator.cs  (optional)
    └── Queries/
        └── {Action}/
            ├── {Action}Query.cs
            └── {Action}QueryHandler.cs
```

## Query Pattern
```csharp
// Query record — no side effects, returns DTO
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

// Handler — inject IApplicationDbContext, use AutoMapper
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        return _mapper.Map<ProductDto>(entity);
    }
}
```

## Command Pattern
```csharp
// Command — mutates state
public class CreateProductCommand : IRequest<ProductDto>
{
    public string NameUk { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public ProductCategory Category { get; init; }
}

// Validator — runs automatically via ValidationBehavior pipeline
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.NameUk).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).IsInEnum();
    }
}
```

## Rules
- Always pass `CancellationToken` to async calls
- Throw `NotFoundException` for missing entities
- Throw `ForbiddenException` for `IsDefault == true` modifications
- Call `SaveChangesAsync` after mutations
- Use `_mapper.Map<TDto>()` for entity-to-DTO conversion
- For entities extending `NamedEntity`: include both `NameUk` and `NameEn` properties in commands/DTOs
