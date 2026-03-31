using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            NameUk = request.NameUk,
            NameEn = request.NameEn,
            Category = request.Category,
            IsDefault = false,
        };

        await context.Products.AddAsync(product, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductDto>(product);
    }
}
