using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace BabyFoodChecklist.McpServer.Tools;

[McpServerToolType]
public sealed class StatisticsTools
{
    [McpServerTool(Name = "get_statistics"), Description(
        "Get overall and per-category progress statistics for the baby's food journey. " +
        "Shows total products, how many have been tried, and progress percentage " +
        "both overall and broken down by each food category.")]
    public static async Task<string> GetStatistics(
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var products = await context.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var entries = await context.UserProductEntries
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var triedProductIds = entries.Where(e => e.Tried).Select(e => e.ProductId).ToHashSet();

        var totalProducts = products.Count;
        var triedProducts = products.Count(p => triedProductIds.Contains(p.Id));
        var overallProgress = totalProducts == 0 ? 0 : Math.Round(triedProducts / (double)totalProducts * 100, 1);

        var sb = new StringBuilder();
        sb.AppendLine("📊 **Baby Food Journey Statistics**");
        sb.AppendLine();
        sb.AppendLine($"Overall: {triedProducts}/{totalProducts} foods tried ({overallProgress}%)");
        sb.AppendLine($"Progress bar: {GetProgressBar(overallProgress)}");
        sb.AppendLine();
        sb.AppendLine("### Per-Category Breakdown:");
        sb.AppendLine();

        var categoryBreakdown = products
            .GroupBy(p => p.Category)
            .OrderBy(g => g.Key);

        foreach (var group in categoryBreakdown)
        {
            var categoryNameEn = CategoryHelper.GetNameEn(group.Key);
            var categoryTotal = group.Count();
            var categoryTried = group.Count(p => triedProductIds.Contains(p.Id));
            var categoryProgress = categoryTotal == 0 ? 0 : Math.Round(categoryTried / (double)categoryTotal * 100, 1);

            sb.AppendLine($"**{categoryNameEn}**: {categoryTried}/{categoryTotal} ({categoryProgress}%) {GetProgressBar(categoryProgress)}");
        }

        // Add rating breakdown
        var ratingGroups = entries
            .Where(e => e.Tried && e.Rating.HasValue)
            .GroupBy(e => e.Rating!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        sb.AppendLine();
        sb.AppendLine("### Rating Summary:");
        sb.AppendLine($"  ❤️ Liked: {ratingGroups.GetValueOrDefault(FoodRating.Liked, 0)}");
        sb.AppendLine($"  😐 Neutral: {ratingGroups.GetValueOrDefault(FoodRating.Neutral, 0)}");
        sb.AppendLine($"  👎 Disliked: {ratingGroups.GetValueOrDefault(FoodRating.Disliked, 0)}");
        sb.AppendLine($"  ⬜ Not rated: {entries.Count(e => e.Tried && !e.Rating.HasValue)}");

        var favoritesCount = entries.Count(e => e.IsFavorite);
        if (favoritesCount > 0)
        {
            sb.AppendLine($"  ⭐ Favorites: {favoritesCount}");
        }

        return sb.ToString();
    }

    private static string GetProgressBar(double percentage)
    {
        const int barLength = 20;
        var filled = (int)Math.Round(percentage / 100 * barLength);
        var empty = barLength - filled;
        return $"[{new string('█', filled)}{new string('░', empty)}]";
    }
}
