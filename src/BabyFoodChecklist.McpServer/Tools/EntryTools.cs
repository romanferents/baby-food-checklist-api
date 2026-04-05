using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace BabyFoodChecklist.McpServer.Tools;

[McpServerToolType]
public sealed class EntryTools
{
    [McpServerTool(Name = "get_tried_foods"), Description(
        "Get all foods the baby has tried, including ratings, dates, and notes. " +
        "Shows when each food was first tried, how the baby rated it (Liked/Neutral/Disliked), " +
        "any reaction notes, and whether it's marked as a favorite.")]
    public static async Task<string> GetTriedFoods(
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var entries = await context.UserProductEntries
            .AsNoTracking()
            .Include(e => e.Product)
            .Where(e => e.Tried)
            .OrderByDescending(e => e.FirstTriedAt)
            .ToListAsync(cancellationToken);

        if (entries.Count == 0)
        {
            return "No foods have been tried yet. The baby's food journey hasn't started!";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"✅ Baby has tried {entries.Count} food(s):");
        sb.AppendLine();

        foreach (var entry in entries)
        {
            var ratingEmoji = entry.Rating switch
            {
                FoodRating.Liked => "❤️ Liked",
                FoodRating.Neutral => "😐 Neutral",
                FoodRating.Disliked => "👎 Disliked",
                _ => "⬜ Not rated",
            };

            var favoriteTag = entry.IsFavorite ? " ⭐" : "";
            var categoryNameEn = CategoryHelper.GetNameEn(entry.Product.Category);

            sb.AppendLine($"• **{entry.Product.NameEn}** ({entry.Product.NameUk}){favoriteTag}");
            sb.AppendLine($"  Category: {categoryNameEn}");
            sb.AppendLine($"  Rating: {ratingEmoji}");

            if (entry.FirstTriedAt.HasValue)
            {
                sb.AppendLine($"  First tried: {entry.FirstTriedAt.Value:yyyy-MM-dd}");
            }

            if (!string.IsNullOrWhiteSpace(entry.ReactionNote))
            {
                sb.AppendLine($"  ⚠️ Reaction: {entry.ReactionNote}");
            }

            if (!string.IsNullOrWhiteSpace(entry.Notes))
            {
                sb.AppendLine($"  Notes: {entry.Notes}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_untried_foods"), Description(
        "Get all foods the baby has NOT tried yet. " +
        "Useful for finding new foods to introduce. " +
        "Optionally filter by category to focus on specific food groups.")]
    public static async Task<string> GetUntriedFoods(
        IApplicationDbContext context,
        [Description("Optional category filter (e.g., 'Fruits', 'Vegetables'). Leave empty for all categories.")] string? category,
        CancellationToken cancellationToken)
    {
        var triedProductIds = await context.UserProductEntries
            .AsNoTracking()
            .Where(e => e.Tried)
            .Select(e => e.ProductId)
            .ToListAsync(cancellationToken);

        var triedSet = triedProductIds.ToHashSet();

        var query = context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(category) &&
            Enum.TryParse<ProductCategory>(category, ignoreCase: true, out var parsedCategory))
        {
            query = query.Where(p => p.Category == parsedCategory);
        }

        var untriedProducts = await query
            .Where(p => !triedSet.Contains(p.Id))
            .OrderBy(p => p.Category)
            .ThenBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);

        if (untriedProducts.Count == 0)
        {
            return category != null
                ? $"🎉 All foods in the {category} category have been tried!"
                : "🎉 Amazing! All foods have been tried!";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"🆕 {untriedProducts.Count} food(s) not yet tried:");
        sb.AppendLine();

        var grouped = untriedProducts.GroupBy(p => p.Category);

        foreach (var group in grouped)
        {
            var categoryNameEn = CategoryHelper.GetNameEn(group.Key);
            sb.AppendLine($"### {categoryNameEn} ({group.Count()} untried)");

            foreach (var product in group)
            {
                sb.AppendLine($"  • {product.NameEn} ({product.NameUk})");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_food_entries_with_reactions"), Description(
        "Get all food entries that have reaction notes recorded. " +
        "This is important for allergy tracking and identifying foods that caused adverse reactions.")]
    public static async Task<string> GetFoodEntriesWithReactions(
        IApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var entries = await context.UserProductEntries
            .AsNoTracking()
            .Include(e => e.Product)
            .Where(e => e.ReactionNote != null && e.ReactionNote != "")
            .OrderByDescending(e => e.FirstTriedAt)
            .ToListAsync(cancellationToken);

        if (entries.Count == 0)
        {
            return "No reaction notes have been recorded for any foods. This is good news! 🎉";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"⚠️ {entries.Count} food(s) with reaction notes:");
        sb.AppendLine();

        foreach (var entry in entries)
        {
            var ratingEmoji = entry.Rating switch
            {
                FoodRating.Liked => "❤️ Liked",
                FoodRating.Neutral => "😐 Neutral",
                FoodRating.Disliked => "👎 Disliked",
                _ => "⬜ Not rated",
            };

            sb.AppendLine($"• **{entry.Product.NameEn}** ({entry.Product.NameUk})");
            sb.AppendLine($"  Rating: {ratingEmoji}");

            if (entry.FirstTriedAt.HasValue)
            {
                sb.AppendLine($"  First tried: {entry.FirstTriedAt.Value:yyyy-MM-dd}");
            }

            sb.AppendLine($"  🔴 Reaction: {entry.ReactionNote}");
            sb.AppendLine();
        }

        sb.AppendLine("⚕️ **Disclaimer**: Always consult your pediatrician about any food reactions or allergies.");
        return sb.ToString();
    }
}
