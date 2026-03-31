using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<ProductDto>
{
    public string NameUk { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public ProductCategory Category { get; init; }
}
