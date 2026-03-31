using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
