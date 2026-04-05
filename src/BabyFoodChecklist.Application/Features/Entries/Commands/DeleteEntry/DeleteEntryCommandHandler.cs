using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Entries.Commands.DeleteEntry;

public class DeleteEntryCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<DeleteEntryCommand>
{
    public async Task Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new ForbiddenException("User must be authenticated.");

        var entry = await context.UserProductEntries
            .FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(UserProductEntry), request.Id);

        context.UserProductEntries.Remove(entry);
        await context.SaveChangesAsync(cancellationToken);
    }
}
