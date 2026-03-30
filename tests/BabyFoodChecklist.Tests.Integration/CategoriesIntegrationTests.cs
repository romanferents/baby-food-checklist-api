using BabyFoodChecklist.Application.Features.Categories.Queries;
using BabyFoodChecklist.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BabyFoodChecklist.Tests.Integration;

/// <summary>
/// Integration tests for the Categories endpoint (no database required).
/// </summary>
public class CategoriesIntegrationTests
{
    [Fact]
    public async Task GetCategories_ReturnsAllNineCategories()
    {
        // Arrange - set up a minimal service collection with application services
        var services = new ServiceCollection();
        services.AddLogging();
        BabyFoodChecklist.Application.DependencyInjection.AddApplicationServices(services);

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();

        // Act
        var result = (await mediator.Send(new GetCategoriesQuery())).ToList();

        // Assert
        result.Should().HaveCount(9);
        result.Should().Contain(c => c.Value == ProductCategory.Vegetables);
        result.Should().Contain(c => c.Value == ProductCategory.Fruits);
        result.Should().AllSatisfy(c =>
        {
            c.NameEn.Should().NotBeNullOrWhiteSpace();
            c.NameUk.Should().NotBeNullOrWhiteSpace();
        });
    }
}
