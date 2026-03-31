using BabyFoodChecklist.API.Controllers;
using BabyFoodChecklist.Application.Features.Products.Commands.CreateProduct;
using BabyFoodChecklist.Application.Features.Products.Commands.DeleteProduct;
using BabyFoodChecklist.Application.Features.Products.Queries.GetProductById;

namespace BabyFoodChecklist.Tests.Controllers;

[TestFixture]
public class ProductsControllerTests
{
    private Mock<ISender> _senderMock = null!;
    private ProductsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _senderMock = new Mock<ISender>();
        _controller = new ProductsController(_senderMock.Object);
    }

    [Test]
    public async Task GetById_ReturnsOk_WhenProductExists()
    {
        var id = Guid.NewGuid();
        var dto = new ProductDto { Id = id, NameUk = "Морква", NameEn = "Carrot", Category = ProductCategory.Vegetables };
        _senderMock.Setup(s => s.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.GetById(id, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(dto);
    }

    [Test]
    public async Task Create_ReturnsCreatedAtAction_WhenCommandIsValid()
    {
        var command = new CreateProductCommand { NameUk = "Тест", NameEn = "Test", Category = ProductCategory.Other };
        var dto = new ProductDto { Id = Guid.NewGuid(), NameUk = "Тест", NameEn = "Test" };
        _senderMock.Setup(s => s.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _controller.Create(command, CancellationToken.None);

        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test]
    public async Task Delete_ReturnsNoContent_WhenProductDeleted()
    {
        var id = Guid.NewGuid();
        _senderMock.Setup(s => s.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(MediatR.Unit.Value));

        var result = await _controller.Delete(id, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }
}
