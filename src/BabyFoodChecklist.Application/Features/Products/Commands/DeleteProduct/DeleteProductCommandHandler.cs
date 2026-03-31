using BabyFoodChecklist.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        if (product.IsDefault)
        {
            throw new ForbiddenException("Default products cannot be deleted.");
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);
    }
}
