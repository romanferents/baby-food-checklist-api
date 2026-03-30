using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Domain.Interfaces;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Entries.Commands.DeleteEntry;

/// <summary>
/// Command for deleting a user product entry.
/// </summary>
public record DeleteEntryCommand(Guid Id) : IRequest;

/// <summary>
/// Handler for <see cref="DeleteEntryCommand"/>.
/// </summary>
public class DeleteEntryCommandHandler : IRequestHandler<DeleteEntryCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="DeleteEntryCommandHandler"/>.</summary>
    public DeleteEntryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _unitOfWork.Entries.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.UserProductEntry), request.Id);

        _unitOfWork.Entries.Remove(entry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
