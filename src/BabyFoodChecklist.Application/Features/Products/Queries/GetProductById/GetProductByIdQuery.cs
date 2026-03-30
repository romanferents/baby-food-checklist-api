using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Interfaces;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// Query for retrieving a single product by its identifier.
/// </summary>
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

/// <summary>
/// Handler for <see cref="GetProductByIdQuery"/>.
/// </summary>
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="GetProductByIdQueryHandler"/>.</summary>
    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.Id);

        return new ProductDto
        {
            Id = product.Id,
            NameUk = product.NameUk,
            NameEn = product.NameEn,
            Category = product.Category,
            CategoryNameEn = CategoryHelper.GetNameEn(product.Category),
            CategoryNameUk = CategoryHelper.GetNameUk(product.Category),
            IsDefault = product.IsDefault,
            SortOrder = product.SortOrder,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
