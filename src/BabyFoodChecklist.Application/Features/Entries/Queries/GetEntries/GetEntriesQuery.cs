using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Application.Common.Models;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Entries.Queries.GetEntries;

/// <summary>
/// Query for retrieving a paged list of user product entries.
/// </summary>
public record GetEntriesQuery(
    int Page = 1,
    int PageSize = 20,
    bool? Tried = null,
    bool? IsFavorite = null,
    ProductCategory? Category = null) : IRequest<PagedResult<UserProductEntryDto>>;

/// <summary>
/// Handler for <see cref="GetEntriesQuery"/>.
/// </summary>
public class GetEntriesQueryHandler : IRequestHandler<GetEntriesQuery, PagedResult<UserProductEntryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="GetEntriesQueryHandler"/>.</summary>
    public GetEntriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<PagedResult<UserProductEntryDto>> Handle(GetEntriesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Entries.GetPagedAsync(
            request.Page, request.PageSize, request.Tried, request.IsFavorite, request.Category, cancellationToken);

        return new PagedResult<UserProductEntryDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static UserProductEntryDto MapToDto(Domain.Entities.UserProductEntry e)
    {
        var dto = new UserProductEntryDto
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

        if (e.Product != null)
        {
            dto.Product = new ProductDto
            {
                Id = e.Product.Id,
                NameUk = e.Product.NameUk,
                NameEn = e.Product.NameEn,
                Category = e.Product.Category,
                CategoryNameEn = CategoryHelper.GetNameEn(e.Product.Category),
                CategoryNameUk = CategoryHelper.GetNameUk(e.Product.Category),
                IsDefault = e.Product.IsDefault,
                SortOrder = e.Product.SortOrder,
                CreatedAt = e.Product.CreatedAt,
                UpdatedAt = e.Product.UpdatedAt
            };
        }

        return dto;
    }
}
