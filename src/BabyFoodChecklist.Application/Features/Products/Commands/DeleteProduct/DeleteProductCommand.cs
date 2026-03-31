using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Domain.Interfaces;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Command for deleting a custom product.
/// </summary>
public record DeleteProductCommand(Guid Id) : IRequest;

/// <summary>
/// Handler for <see cref="DeleteProductCommand"/>.
/// </summary>
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="DeleteProductCommandHandler"/>.</summary>
    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.Id);

        if (product.IsDefault)
            throw new ForbiddenException("Default products cannot be deleted.");

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
