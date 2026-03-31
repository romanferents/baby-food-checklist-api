using BabyFoodChecklist.Application.Features.Categories.Queries.GetCategories;

namespace BabyFoodChecklist.Tests.Features.Categories;

[TestFixture]
public class GetCategoriesQueryHandlerTests
{
    [Test]
    public async Task Handle_ReturnsAllCategories_WhenCalled()
    {
        var handler = new GetCategoriesQueryHandler();

        var result = await handler.Handle(new GetCategoriesQuery(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(Enum.GetValues<ProductCategory>().Length);
        result.All(c => !string.IsNullOrWhiteSpace(c.NameUk)).Should().BeTrue();
        result.All(c => !string.IsNullOrWhiteSpace(c.NameEn)).Should().BeTrue();
    }

    [Test]
    public void GetNameUk_ReturnsUkrainianName_WhenCategoryIsVegetables()
    {
        var name = CategoryHelper.GetNameUk(ProductCategory.Vegetables);

        name.Should().Be("Овочі та зелень");
    }

    [Test]
    public void GetNameEn_ReturnsEnglishName_WhenCategoryIsFruits()
    {
        var name = CategoryHelper.GetNameEn(ProductCategory.Fruits);

        name.Should().Be("Fruits");
    }
}
