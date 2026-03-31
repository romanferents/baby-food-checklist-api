using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Features.Products.Commands.CreateProduct;
using BabyFoodChecklist.Application.Features.Products.Commands.DeleteProduct;
using BabyFoodChecklist.Application.Features.Products.Commands.UpdateProduct;
using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BabyFoodChecklist.Tests.Unit.Features.Products;

public class ProductCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact]
    public async Task CreateProduct_ValidCommand_ReturnsProductDto()
    {
        // Arrange
        _unitOfWork.Setup(u => u.Products.AddAsync(It.IsAny<Product>(), default))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var handler = new CreateProductCommandHandler(_unitOfWork.Object);
        var command = new CreateProductCommand("Тест", "Test", ProductCategory.Other);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.NameUk.Should().Be("Тест");
        result.NameEn.Should().Be("Test");
        result.Category.Should().Be(ProductCategory.Other);
        result.IsDefault.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateProduct_DefaultProduct_ThrowsForbiddenException()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            NameUk = "Default",
            NameEn = "Default",
            Category = ProductCategory.Vegetables,
            IsDefault = true
        };

        _unitOfWork.Setup(u => u.Products.GetByIdAsync(product.Id, default))
            .ReturnsAsync(product);

        var handler = new UpdateProductCommandHandler(_unitOfWork.Object);
        var command = new UpdateProductCommand(product.Id, "New UK", "New EN", ProductCategory.Fruits);

        // Act
        var act = async () => await handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task DeleteProduct_DefaultProduct_ThrowsForbiddenException()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            NameUk = "Default",
            NameEn = "Default",
            Category = ProductCategory.Vegetables,
            IsDefault = true
        };

        _unitOfWork.Setup(u => u.Products.GetByIdAsync(product.Id, default))
            .ReturnsAsync(product);

        var handler = new DeleteProductCommandHandler(_unitOfWork.Object);

        // Act
        var act = async () => await handler.Handle(new DeleteProductCommand(product.Id), default);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task DeleteProduct_CustomProduct_DeletesSuccessfully()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            NameUk = "Custom",
            NameEn = "Custom",
            Category = ProductCategory.Other,
            IsDefault = false
        };

        _unitOfWork.Setup(u => u.Products.GetByIdAsync(product.Id, default))
            .ReturnsAsync(product);
        _unitOfWork.Setup(u => u.Products.Remove(product));
        _unitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var handler = new DeleteProductCommandHandler(_unitOfWork.Object);

        // Act
        var act = async () => await handler.Handle(new DeleteProductCommand(product.Id), default);

        // Assert
        await act.Should().NotThrowAsync();
        _unitOfWork.Verify(u => u.Products.Remove(product), Times.Once);
    }
}
