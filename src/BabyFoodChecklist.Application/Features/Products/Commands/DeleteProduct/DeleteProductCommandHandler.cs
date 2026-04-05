using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId;

        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id && (p.UserId == null || p.UserId == userId), cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        if (product.IsDefault)
        {
            throw new ForbiddenException("Default products cannot be deleted.");
        }

        if (product.UserId != userId)
        {
            throw new ForbiddenException("You can only delete your own custom products.");
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);
    }
}
