using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<ProductDto>
{
    public Guid Id { get; init; }
    public string NameUk { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public ProductCategory Category { get; init; }
}
