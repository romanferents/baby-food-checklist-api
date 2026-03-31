using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Entries.Commands.UpdateEntry;

/// <summary>
/// Command for updating an existing user product entry.
/// </summary>
public record UpdateEntryCommand(
    Guid Id,
    bool Tried,
    DateTime? FirstTriedAt,
    FoodRating? Rating,
    string? ReactionNote,
    string? Notes,
    bool IsFavorite) : IRequest<UserProductEntryDto>;

/// <summary>
/// Validator for <see cref="UpdateEntryCommand"/>.
/// </summary>
public class UpdateEntryCommandValidator : AbstractValidator<UpdateEntryCommand>
{
    /// <summary>Initializes a new instance of <see cref="UpdateEntryCommandValidator"/>.</summary>
    public UpdateEntryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ReactionNote).MaximumLength(500);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleFor(x => x.Rating).IsInEnum().When(x => x.Rating.HasValue);
    }
}

/// <summary>
/// Handler for <see cref="UpdateEntryCommand"/>.
/// </summary>
public class UpdateEntryCommandHandler : IRequestHandler<UpdateEntryCommand, UserProductEntryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="UpdateEntryCommandHandler"/>.</summary>
    public UpdateEntryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<UserProductEntryDto> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _unitOfWork.Entries.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.UserProductEntry), request.Id);

        entry.Tried = request.Tried;
        entry.FirstTriedAt = request.FirstTriedAt;
        entry.Rating = request.Rating;
        entry.ReactionNote = request.ReactionNote;
        entry.Notes = request.Notes;
        entry.IsFavorite = request.IsFavorite;
        entry.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Entries.Update(entry);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserProductEntryDto
        {
            Id = entry.Id,
            ProductId = entry.ProductId,
            Tried = entry.Tried,
            FirstTriedAt = entry.FirstTriedAt,
            Rating = entry.Rating,
            ReactionNote = entry.ReactionNote,
            Notes = entry.Notes,
            IsFavorite = entry.IsFavorite,
            CreatedAt = entry.CreatedAt,
            UpdatedAt = entry.UpdatedAt
        };
    }
}
