using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Products.Commands.CreateProduct;

/// <summary>
/// Command for creating a new (custom) product.
/// </summary>
public record CreateProductCommand(
    string NameUk,
    string NameEn,
    ProductCategory Category) : IRequest<ProductDto>;

/// <summary>
/// Validator for <see cref="CreateProductCommand"/>.
/// </summary>
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    /// <summary>Initializes a new instance of <see cref="CreateProductCommandValidator"/>.</summary>
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.NameUk).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).IsInEnum();
    }
}

/// <summary>
/// Handler for <see cref="CreateProductCommand"/>.
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="CreateProductCommandHandler"/>.</summary>
    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            NameUk = request.NameUk,
            NameEn = request.NameEn,
            Category = request.Category,
            IsDefault = false
        };

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
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
