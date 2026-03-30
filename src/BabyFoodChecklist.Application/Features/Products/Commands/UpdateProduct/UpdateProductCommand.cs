using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// Command for updating a custom product.
/// </summary>
public record UpdateProductCommand(
    Guid Id,
    string NameUk,
    string NameEn,
    ProductCategory Category) : IRequest<ProductDto>;

/// <summary>
/// Validator for <see cref="UpdateProductCommand"/>.
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    /// <summary>Initializes a new instance of <see cref="UpdateProductCommandValidator"/>.</summary>
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NameUk).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).IsInEnum();
    }
}

/// <summary>
/// Handler for <see cref="UpdateProductCommand"/>.
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="UpdateProductCommandHandler"/>.</summary>
    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.Id);

        if (product.IsDefault)
            throw new ForbiddenException("Default products cannot be modified.");

        product.NameUk = request.NameUk;
        product.NameEn = request.NameEn;
        product.Category = request.Category;
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductDto
        {
            Id = product.Id,
            NameUk = product.NameUk,
            NameEn = product.NameEn,
            Category = product.Category,
            CategoryNameEn = Common.Helpers.CategoryHelper.GetNameEn(product.Category),
            CategoryNameUk = Common.Helpers.CategoryHelper.GetNameUk(product.Category),
            IsDefault = product.IsDefault,
            SortOrder = product.SortOrder,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
