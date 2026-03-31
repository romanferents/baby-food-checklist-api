using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Domain.Enums;
using FluentAssertions;

namespace BabyFoodChecklist.Tests.Unit.Features;

public class CategoryHelperTests
{
    [Theory]
    [InlineData(ProductCategory.Vegetables, "Vegetables & Greens", "Овочі та зелень")]
    [InlineData(ProductCategory.Fruits, "Fruits", "Фрукти")]
    [InlineData(ProductCategory.Grains, "Grains & Legumes", "Цільнозернові, крупи та бобові")]
    [InlineData(ProductCategory.Meat, "Meat & Animal Products", "М'ясо та тваринні продукти")]
    [InlineData(ProductCategory.Fish, "Fish & Seafood", "Риба, молюски, ракоподібні")]
    [InlineData(ProductCategory.Dairy, "Dairy", "Молочні продукти")]
    [InlineData(ProductCategory.NutsSeeds, "Nuts & Seeds", "Горіхи, насіння")]
    [InlineData(ProductCategory.Spices, "Spices", "Спеції")]
    [InlineData(ProductCategory.Other, "Other", "Інше")]
    public void CategoryHelper_ReturnsCorrectNames(ProductCategory category, string expectedEn, string expectedUk)
    {
        CategoryHelper.GetNameEn(category).Should().Be(expectedEn);
        CategoryHelper.GetNameUk(category).Should().Be(expectedUk);
    }
}
