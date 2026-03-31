using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Interfaces;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Entries.Queries.GetEntryByProductId;

/// <summary>
/// Query for retrieving a single user product entry by product identifier.
/// </summary>
public record GetEntryByProductIdQuery(Guid ProductId) : IRequest<UserProductEntryDto>;

/// <summary>
/// Handler for <see cref="GetEntryByProductIdQuery"/>.
/// </summary>
public class GetEntryByProductIdQueryHandler : IRequestHandler<GetEntryByProductIdQuery, UserProductEntryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="GetEntryByProductIdQueryHandler"/>.</summary>
    public GetEntryByProductIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<UserProductEntryDto> Handle(GetEntryByProductIdQuery request, CancellationToken cancellationToken)
    {
        var entry = await _unitOfWork.Entries.GetByProductIdAsync(request.ProductId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.UserProductEntry), request.ProductId);

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
            UpdatedAt = entry.UpdatedAt,
            Product = entry.Product == null ? null : new ProductDto
            {
                Id = entry.Product.Id,
                NameUk = entry.Product.NameUk,
                NameEn = entry.Product.NameEn,
                Category = entry.Product.Category,
                CategoryNameEn = CategoryHelper.GetNameEn(entry.Product.Category),
                CategoryNameUk = CategoryHelper.GetNameUk(entry.Product.Category),
                IsDefault = entry.Product.IsDefault,
                SortOrder = entry.Product.SortOrder,
                CreatedAt = entry.Product.CreatedAt,
                UpdatedAt = entry.Product.UpdatedAt
            }
        };
    }
}
