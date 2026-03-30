using BabyFoodChecklist.Infrastructure.Persistence;
using FluentAssertions;

namespace BabyFoodChecklist.Tests.Unit.Features;

public class DbSeederTests
{
    [Fact]
    public void GetDefaultProducts_Returns100OrMoreProducts()
    {
        var products = DbSeeder.GetDefaultProducts().ToList();
        products.Should().HaveCountGreaterThanOrEqualTo(100);
    }

    [Fact]
    public void GetDefaultProducts_AllHaveRequiredFields()
    {
        var products = DbSeeder.GetDefaultProducts().ToList();
        products.Should().AllSatisfy(p =>
        {
            p.NameUk.Should().NotBeNullOrWhiteSpace();
            p.NameEn.Should().NotBeNullOrWhiteSpace();
            p.IsDefault.Should().BeTrue();
            p.SortOrder.Should().BeGreaterThan(0);
        });
    }

    [Fact]
    public void GetDefaultProducts_SortOrderIsUnique()
    {
        var products = DbSeeder.GetDefaultProducts().ToList();
        var sortOrders = products.Select(p => p.SortOrder).ToList();
        sortOrders.Should().OnlyHaveUniqueItems();
    }
}
