using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = Enum.GetValues<ProductCategory>()
            .Select(c => new CategoryDto
            {
                Value = (int)c,
                NameUk = CategoryHelper.GetNameUk(c),
                NameEn = CategoryHelper.GetNameEn(c),
            })
            .ToList();

        return Task.FromResult<IReadOnlyList<CategoryDto>>(categories);
    }
}
