using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Entries.Commands.CreateEntry;

/// <summary>
/// Command for creating or updating a user product entry.
/// </summary>
public record CreateEntryCommand(
    Guid ProductId,
    bool Tried,
    DateTime? FirstTriedAt,
    FoodRating? Rating,
    string? ReactionNote,
    string? Notes,
    bool IsFavorite) : IRequest<UserProductEntryDto>;

/// <summary>
/// Validator for <see cref="CreateEntryCommand"/>.
/// </summary>
public class CreateEntryCommandValidator : AbstractValidator<CreateEntryCommand>
{
    /// <summary>Initializes a new instance of <see cref="CreateEntryCommandValidator"/>.</summary>
    public CreateEntryCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.ReactionNote).MaximumLength(500);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleFor(x => x.Rating).IsInEnum().When(x => x.Rating.HasValue);
    }
}

/// <summary>
/// Handler for <see cref="CreateEntryCommand"/>.
/// </summary>
public class CreateEntryCommandHandler : IRequestHandler<CreateEntryCommand, UserProductEntryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="CreateEntryCommandHandler"/>.</summary>
    public CreateEntryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<UserProductEntryDto> Handle(CreateEntryCommand request, CancellationToken cancellationToken)
    {
        _ = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.ProductId);

        // Check if entry already exists; if so, treat as upsert
        var existing = await _unitOfWork.Entries.GetByProductIdAsync(request.ProductId, cancellationToken);
        if (existing != null)
        {
            existing.Tried = request.Tried;
            existing.FirstTriedAt = request.FirstTriedAt;
            existing.Rating = request.Rating;
            existing.ReactionNote = request.ReactionNote;
            existing.Notes = request.Notes;
            existing.IsFavorite = request.IsFavorite;
            existing.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Entries.Update(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MapToDto(existing);
        }

        var entry = new UserProductEntry
        {
            ProductId = request.ProductId,
            Tried = request.Tried,
            FirstTriedAt = request.FirstTriedAt,
            Rating = request.Rating,
            ReactionNote = request.ReactionNote,
            Notes = request.Notes,
            IsFavorite = request.IsFavorite
        };

        await _unitOfWork.Entries.AddAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(entry);
    }

    private static UserProductEntryDto MapToDto(UserProductEntry e) => new()
    {
        Id = e.Id,
        ProductId = e.ProductId,
        Tried = e.Tried,
        FirstTriedAt = e.FirstTriedAt,
        Rating = e.Rating,
        ReactionNote = e.ReactionNote,
        Notes = e.Notes,
        IsFavorite = e.IsFavorite,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
