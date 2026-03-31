using AutoMapper;
using BabyFoodChecklist.Application.Common.Exceptions;
using BabyFoodChecklist.Application.Features.Products.Queries.GetProductById;
using BabyFoodChecklist.Application.MappingProfiles;
using Microsoft.Extensions.Logging.Abstractions;

namespace BabyFoodChecklist.Tests.Features.Products.Queries;

[TestFixture]
public class GetProductByIdQueryHandlerTests
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
    public async Task Handle_ReturnsProductDto_WhenProductExists()
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            NameUk = "Морква",
            NameEn = "Carrot",
            Category = ProductCategory.Vegetables,
            IsDefault = true,
            SortOrder = 1,
        };
        var products = new List<Product> { product }.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Products).Returns(products.Object);

        var handler = new GetProductByIdQueryHandler(_contextMock.Object, _mapper);

        var result = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.NameUk.Should().Be("Морква");
        result.NameEn.Should().Be("Carrot");
    }

    [Test]
    public async Task Handle_ThrowsNotFoundException_WhenProductDoesNotExist()
    {
        var products = new List<Product>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(c => c.Products).Returns(products.Object);

        var handler = new GetProductByIdQueryHandler(_contextMock.Object, _mapper);

        var act = async () => await handler.Handle(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
