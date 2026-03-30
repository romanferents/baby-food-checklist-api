using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Enums;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Categories.Queries;

/// <summary>
/// Query for retrieving all product categories with localised names.
/// </summary>
public record GetCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;

/// <summary>
/// Handler for <see cref="GetCategoriesQuery"/>.
/// </summary>
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
{
    /// <inheritdoc />
    public Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = Enum.GetValues<ProductCategory>()
            .Select(cat => new CategoryDto
            {
                Value = cat,
                NameEn = CategoryHelper.GetNameEn(cat),
                NameUk = CategoryHelper.GetNameUk(cat)
            });

        return Task.FromResult(categories);
    }
}
