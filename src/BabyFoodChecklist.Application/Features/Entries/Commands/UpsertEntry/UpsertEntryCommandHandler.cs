using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Entries.Commands.UpsertEntry;

public class UpsertEntryCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpsertEntryCommand, UserProductEntryDto>
{
    public async Task<UserProductEntryDto> Handle(UpsertEntryCommand request, CancellationToken cancellationToken)
    {
        var productExists = await context.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
        if (!productExists)
            throw new NotFoundException(nameof(Product), request.ProductId);

        var entry = await context.UserProductEntries
            .Include(e => e.Product)
            .FirstOrDefaultAsync(e => e.ProductId == request.ProductId, cancellationToken);

        if (entry is null)
        {
            entry = new UserProductEntry { ProductId = request.ProductId };
            await context.UserProductEntries.AddAsync(entry, cancellationToken);
        }

        entry.Tried = request.Tried;
        entry.FirstTriedAt = request.FirstTriedAt;
        entry.Rating = request.Rating;
        entry.ReactionNote = request.ReactionNote;
        entry.Notes = request.Notes;
        entry.IsFavorite = request.IsFavorite;

        await context.SaveChangesAsync(cancellationToken);

        var result = await context.UserProductEntries
            .Include(e => e.Product)
            .FirstAsync(e => e.ProductId == request.ProductId, cancellationToken);

        return mapper.Map<UserProductEntryDto>(result);
    }
}
