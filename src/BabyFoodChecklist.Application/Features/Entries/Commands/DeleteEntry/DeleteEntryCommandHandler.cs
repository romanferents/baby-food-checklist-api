using BabyFoodChecklist.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Entries.Commands.DeleteEntry;

public class DeleteEntryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteEntryCommand>
{
    public async Task Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await context.UserProductEntries.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(UserProductEntry), request.Id);

        context.UserProductEntries.Remove(entry);
        await context.SaveChangesAsync(cancellationToken);
    }
}
