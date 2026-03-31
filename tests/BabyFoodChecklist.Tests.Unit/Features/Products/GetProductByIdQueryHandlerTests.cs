using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Features.Products.Queries.GetProductById;
using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BabyFoodChecklist.Tests.Unit.Features.Products;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact]
    public async Task Handle_ExistingProduct_ReturnsProductDto()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            NameUk = "Морква",
            NameEn = "Carrot",
            Category = ProductCategory.Vegetables,
            IsDefault = true,
            SortOrder = 1
        };

        _unitOfWork.Setup(u => u.Products.GetByIdAsync(product.Id, default))
            .ReturnsAsync(product);

        var handler = new GetProductByIdQueryHandler(_unitOfWork.Object);

        // Act
        var result = await handler.Handle(new GetProductByIdQuery(product.Id), default);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.NameUk.Should().Be("Морква");
        result.NameEn.Should().Be("Carrot");
        result.Category.Should().Be(ProductCategory.Vegetables);
        result.IsDefault.Should().BeTrue();
        result.CategoryNameEn.Should().Be("Vegetables & Greens");
        result.CategoryNameUk.Should().Be("Овочі та зелень");
    }

    [Fact]
    public async Task Handle_NonExistingProduct_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _unitOfWork.Setup(u => u.Products.GetByIdAsync(id, default))
            .ReturnsAsync((Product?)null);

        var handler = new GetProductByIdQueryHandler(_unitOfWork.Object);

        // Act
        var act = async () => await handler.Handle(new GetProductByIdQuery(id), default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
