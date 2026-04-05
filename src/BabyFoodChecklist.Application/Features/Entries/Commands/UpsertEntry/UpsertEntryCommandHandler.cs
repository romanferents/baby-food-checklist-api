using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Common.Interfaces;
using BabyFoodChecklist.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Entries.Commands.UpsertEntry;

public class UpsertEntryCommandHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser)
    : IRequestHandler<UpsertEntryCommand, UserProductEntryDto>
{
    public async Task<UserProductEntryDto> Handle(UpsertEntryCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new ForbiddenException("User must be authenticated.");

        var productExists = await context.Products
            .AnyAsync(p => p.Id == request.ProductId && (p.UserId == null || p.UserId == userId), cancellationToken);
        if (!productExists)
            throw new NotFoundException(nameof(Product), request.ProductId);

        var entry = await context.UserProductEntries
            .Include(e => e.Product)
            .FirstOrDefaultAsync(e => e.ProductId == request.ProductId && e.UserId == userId, cancellationToken);

        if (entry is null)
        {
            entry = new UserProductEntry { ProductId = request.ProductId, UserId = userId };
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
            .FirstAsync(e => e.ProductId == request.ProductId && e.UserId == userId, cancellationToken);

        return mapper.Map<UserProductEntryDto>(result);
    }
}
