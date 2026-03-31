using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        if (product.IsDefault)
        {
            throw new ForbiddenException("Default products cannot be modified.");
        }

        product.NameUk = request.NameUk;
        product.NameEn = request.NameEn;
        product.Category = request.Category;

        await context.SaveChangesAsync(cancellationToken);
        return mapper.Map<ProductDto>(product);
    }
}
