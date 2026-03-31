using AutoMapper;
using BabyFoodChecklist.Application.Features.Products.Commands.CreateProduct;
using BabyFoodChecklist.Application.MappingProfiles;
using Microsoft.Extensions.Logging.Abstractions;

namespace BabyFoodChecklist.Tests.Features.Products.Commands;

[TestFixture]
public class CreateProductCommandHandlerTests
{
    private Mock<IApplicationDbContext> _contextMock = null!;
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        var config = new MapperConfiguration(c => c.AddProfile<ProductMappingProfile>(), NullLoggerFactory.Instance);
        _mapper = config.CreateMapper();
    }

    [Test]
    public async Task Handle_ReturnsCreatedProduct_WhenCommandIsValid()
    {
        var products = new List<Product>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Products).Returns(products.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new CreateProductCommand
        {
            NameUk = "Тест",
            NameEn = "Test",
            Category = ProductCategory.Other,
        };

        var handler = new CreateProductCommandHandler(_contextMock.Object, _mapper);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.NameUk.Should().Be("Тест");
        result.NameEn.Should().Be("Test");
        result.IsDefault.Should().BeFalse();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
