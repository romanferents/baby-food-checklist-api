using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Application.DTOs;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using MediatR;

namespace BabyFoodChecklist.Application.Features.Statistics.Queries;

/// <summary>
/// Query for retrieving overall progress statistics.
/// </summary>
public record GetStatisticsQuery : IRequest<StatisticsDto>;

/// <summary>
/// Handler for <see cref="GetStatisticsQuery"/>.
/// </summary>
public class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, StatisticsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes a new instance of <see cref="GetStatisticsQueryHandler"/>.</summary>
    public GetStatisticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<StatisticsDto> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
    {
        // Load all products (no pagination for stats)
        var (allProducts, totalProducts) = await _unitOfWork.Products.GetPagedAsync(1, int.MaxValue, cancellationToken: cancellationToken);
        var (allEntries, _) = await _unitOfWork.Entries.GetPagedAsync(1, int.MaxValue, cancellationToken: cancellationToken);

        var entriesByProduct = allEntries.ToDictionary(e => e.ProductId);

        var triedCount = allEntries.Count(e => e.Tried);
        var favoritesCount = allEntries.Count(e => e.IsFavorite);
        var percentage = totalProducts > 0 ? Math.Round((double)triedCount / totalProducts * 100, 1) : 0;

        var byCategory = Enum.GetValues<ProductCategory>()
            .Select(cat =>
            {
                var categoryProducts = allProducts.Where(p => p.Category == cat).ToList();
                var categoryTried = categoryProducts.Count(p =>
                    entriesByProduct.TryGetValue(p.Id, out var entry) && entry.Tried);
                var categoryTotal = categoryProducts.Count;
                return new CategoryStatisticsDto
                {
                    Category = cat,
                    CategoryNameEn = CategoryHelper.GetNameEn(cat),
                    CategoryNameUk = CategoryHelper.GetNameUk(cat),
                    Total = categoryTotal,
                    Tried = categoryTried,
                    Percentage = categoryTotal > 0 ? Math.Round((double)categoryTried / categoryTotal * 100, 1) : 0
                };
            })
            .Where(c => c.Total > 0)
            .ToList();

        return new StatisticsDto
        {
            TotalProducts = totalProducts,
            TriedCount = triedCount,
            PercentageTried = percentage,
            FavoritesCount = favoritesCount,
            ByCategory = byCategory
        };
    }
}
