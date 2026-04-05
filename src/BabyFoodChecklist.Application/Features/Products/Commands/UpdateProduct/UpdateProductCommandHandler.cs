using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Common.Interfaces;
using BabyFoodChecklist.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUser)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id && (p.UserId == null || p.UserId == userId), cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        if (product.IsDefault)
        {
            throw new ForbiddenException("Default products cannot be modified.");
        }

        if (product.UserId != userId)
        {
            throw new ForbiddenException("You can only modify your own custom products.");
        }

        product.NameUk = request.NameUk;
        product.NameEn = request.NameEn;
        product.Category = request.Category;

        await context.SaveChangesAsync(cancellationToken);
        return mapper.Map<ProductDto>(product);
    }
}
