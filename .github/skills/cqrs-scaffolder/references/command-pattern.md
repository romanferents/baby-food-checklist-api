# Command Pattern Reference

## File: `{Name}Command.cs`
```csharp
using MediatR;
using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Application.Features.{Feature}.Commands.{Name};

public class {Name}Command : IRequest<{ReturnDto}>
{
    public string NameUk { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    // Add other properties as needed
}
```

## File: `{Name}CommandHandler.cs`
```csharp
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Interfaces;

namespace BabyFoodChecklist.Application.Features.{Feature}.Commands.{Name};

public class {Name}CommandHandler : IRequestHandler<{Name}Command, {ReturnDto}>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public {Name}CommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<{ReturnDto}> Handle({Name}Command request, CancellationToken cancellationToken)
    {
        // 1. Create entity
        var entity = new {Entity}
        {
            NameUk = request.NameUk,
            NameEn = request.NameEn,
            // Map other properties
        };

        // 2. Persist
        await _context.{DbSet}.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // 3. Map and return
        return _mapper.Map<{ReturnDto}>(entity);
    }
}
```

## File: `{Name}CommandValidator.cs`
```csharp
using FluentValidation;

namespace BabyFoodChecklist.Application.Features.{Feature}.Commands.{Name};

public class {Name}CommandValidator : AbstractValidator<{Name}Command>
{
    public {Name}CommandValidator()
    {
        RuleFor(x => x.NameUk)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.NameEn)
            .NotEmpty()
            .MaximumLength(200);

        // Add more rules as needed
    }
}
```

## Business Rules
- Check `IsDefault` before modifying/deleting: throw `ForbiddenException` if true
- Upsert pattern: check if entity exists by key, then create or update accordingly
- Always call `SaveChangesAsync` with `cancellationToken`

## Controller Endpoints
```csharp
// Create
[HttpPost]
public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken cancellationToken)
{
    var result = await _sender.Send(command, cancellationToken);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}

// Update
[HttpPut("{id:guid}")]
public async Task<IActionResult> Update(Guid id, UpdateProductCommand command, CancellationToken cancellationToken)
{
    command.Id = id;
    var result = await _sender.Send(command, cancellationToken);
    return Ok(result);
}

// Delete
[HttpDelete("{id:guid}")]
public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
{
    await _sender.Send(new DeleteProductCommand(id), cancellationToken);
    return NoContent();
}
```
