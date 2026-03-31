using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Application.Features.Statistics.Queries.GetStatistics;

public class GetStatisticsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetStatisticsQuery, StatisticsDto>
{
    public async Task<StatisticsDto> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
    {
        var products = await context.Products.AsNoTracking().ToListAsync(cancellationToken);
        var entries = await context.UserProductEntries.AsNoTracking().ToListAsync(cancellationToken);

        var triedProductIds = entries.Where(e => e.Tried).Select(e => e.ProductId).ToHashSet();

        var totalProducts = products.Count;
        var triedProducts = products.Count(p => triedProductIds.Contains(p.Id));

        var categoryBreakdown = products
            .GroupBy(p => p.Category)
            .Select(g => new CategoryStatisticsDto
            {
                Category = g.Key,
                CategoryNameUk = CategoryHelper.GetNameUk(g.Key),
                CategoryNameEn = CategoryHelper.GetNameEn(g.Key),
                TotalProducts = g.Count(),
                TriedProducts = g.Count(p => triedProductIds.Contains(p.Id)),
                ProgressPercentage = g.Count() == 0 ? 0
                    : Math.Round(g.Count(p => triedProductIds.Contains(p.Id)) / (double)g.Count() * 100, 1),
            })
            .OrderBy(c => c.Category)
            .ToList();

        return new StatisticsDto
        {
            TotalProducts = totalProducts,
            TriedProducts = triedProducts,
            ProgressPercentage = totalProducts == 0 ? 0 : Math.Round(triedProducts / (double)totalProducts * 100, 1),
            CategoryBreakdown = categoryBreakdown,
        };
    }
}
