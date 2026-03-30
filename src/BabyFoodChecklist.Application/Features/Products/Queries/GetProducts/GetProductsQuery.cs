using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Products.Queries.GetProducts;

/// <summary>
/// Query for retrieving a paged list of products.
/// </summary>
public record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    ProductCategory? Category = null,
    string? SearchTerm = null) : IRequest<Common.Models.PagedResult<ProductDto>>;

/// <summary>
/// Handler for <see cref="GetProductsQuery"/>.
/// </summary>
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Common.Models.PagedResult<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="GetProductsQueryHandler"/>.</summary>
    public GetProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Common.Models.PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Products.GetPagedAsync(
            request.Page, request.PageSize, request.Category, request.SearchTerm, cancellationToken);

        return new Common.Models.PagedResult<ProductDto>
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static ProductDto MapToDto(Domain.Entities.Product p) => new()
    {
        Id = p.Id,
        NameUk = p.NameUk,
        NameEn = p.NameEn,
        Category = p.Category,
        CategoryNameEn = CategoryHelper.GetNameEn(p.Category),
        CategoryNameUk = CategoryHelper.GetNameUk(p.Category),
        IsDefault = p.IsDefault,
        SortOrder = p.SortOrder,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}
