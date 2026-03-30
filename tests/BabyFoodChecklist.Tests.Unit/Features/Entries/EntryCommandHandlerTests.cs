using BabyFoodChecklist.Application.Features.Entries.Commands.CreateEntry;
using BabyFoodChecklist.Application.Features.Entries.Commands.DeleteEntry;
using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BabyFoodChecklist.Tests.Unit.Features.Entries;

public class EntryCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    [Fact]
    public async Task CreateEntry_NewEntry_CreatesAndReturnsDto()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, NameUk = "Морква", NameEn = "Carrot", Category = ProductCategory.Vegetables };

        _unitOfWork.Setup(u => u.Products.GetByIdAsync(productId, default))
            .ReturnsAsync(product);
        _unitOfWork.Setup(u => u.Entries.GetByProductIdAsync(productId, default))
            .ReturnsAsync((UserProductEntry?)null);
        _unitOfWork.Setup(u => u.Entries.AddAsync(It.IsAny<UserProductEntry>(), default))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var handler = new CreateEntryCommandHandler(_unitOfWork.Object);
        var command = new CreateEntryCommand(productId, true, DateTime.UtcNow, FoodRating.Liked, null, null, false);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(productId);
        result.Tried.Should().BeTrue();
        result.Rating.Should().Be(FoodRating.Liked);
    }

    [Fact]
    public async Task CreateEntry_ExistingEntry_UpdatesEntry()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, NameUk = "Морква", NameEn = "Carrot", Category = ProductCategory.Vegetables };
        var existing = new UserProductEntry { Id = Guid.NewGuid(), ProductId = productId, Tried = false };

        _unitOfWork.Setup(u => u.Products.GetByIdAsync(productId, default))
            .ReturnsAsync(product);
        _unitOfWork.Setup(u => u.Entries.GetByProductIdAsync(productId, default))
            .ReturnsAsync(existing);
        _unitOfWork.Setup(u => u.Entries.Update(existing));
        _unitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var handler = new CreateEntryCommandHandler(_unitOfWork.Object);
        var command = new CreateEntryCommand(productId, true, DateTime.UtcNow, FoodRating.Neutral, null, "Note", true);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.Tried.Should().BeTrue();
        result.IsFavorite.Should().BeTrue();
        _unitOfWork.Verify(u => u.Entries.Update(existing), Times.Once);
    }

    [Fact]
    public async Task DeleteEntry_ExistingEntry_DeletesSuccessfully()
    {
        // Arrange
        var entryId = Guid.NewGuid();
        var entry = new UserProductEntry { Id = entryId, ProductId = Guid.NewGuid() };

        _unitOfWork.Setup(u => u.Entries.GetByIdAsync(entryId, default))
            .ReturnsAsync(entry);
        _unitOfWork.Setup(u => u.Entries.Remove(entry));
        _unitOfWork.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);

        var handler = new DeleteEntryCommandHandler(_unitOfWork.Object);

        // Act
        var act = async () => await handler.Handle(new DeleteEntryCommand(entryId), default);

        // Assert
        await act.Should().NotThrowAsync();
        _unitOfWork.Verify(u => u.Entries.Remove(entry), Times.Once);
    }
}
