# Query Pattern Reference

## File: `{Name}Query.cs`
```csharp
using MediatR;

namespace BabyFoodChecklist.Application.Features.{Feature}.Queries.{Name};

public record {Name}Query({Parameters}) : IRequest<{ReturnDto}>;
```

### Examples
```csharp
// Simple by-ID query
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

// Parameterless query
public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;

// Query returning a list
public record GetStatisticsQuery : IRequest<StatisticsDto>;
```

## File: `{Name}QueryHandler.cs`
```csharp
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Domain.Interfaces;

namespace BabyFoodChecklist.Application.Features.{Feature}.Queries.{Name};

public class {Name}QueryHandler : IRequestHandler<{Name}Query, {ReturnDto}>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public {Name}QueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<{ReturnDto}> Handle({Name}Query request, CancellationToken cancellationToken)
    {
        // 1. Query the data
        var entity = await _context.{DbSet}
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof({Entity}), request.Id);

        // 2. Map and return
        return _mapper.Map<{ReturnDto}>(entity);
    }
}
```

## Controller Endpoint
```csharp
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
{
    var result = await _sender.Send(new GetProductByIdQuery(id), cancellationToken);
    return Ok(result);
}
```
