using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;
