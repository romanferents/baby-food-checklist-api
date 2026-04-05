using BabyFoodChecklist.Application.Common.Helpers;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace BabyFoodChecklist.McpServer.Tools;

[McpServerToolType]
public sealed class ProductTools
{
    [McpServerTool(Name = "get_all_products"), Description(
        "Get all baby food products from the database, grouped by category. " +
        "Returns bilingual names (Ukrainian and English) for each product and category. " +
        "Use this to see the complete list of 126+ foods across 9 categories. " +
        "Requires userId to show user-specific custom products alongside default products.")]
    public static async Task<string> GetAllProducts(
        IApplicationDbContext context,
        [Description("The user ID (GUID) to filter products for. Shows default products + user's custom products.")] string userId,
        CancellationToken cancellationToken)
    {
        var (isValid, parsedUserId, error) = UserIdParser.Parse(userId);
        if (!isValid) return error!;

        var products = await context.Products
            .AsNoTracking()
            .Where(p => p.UserId == null || p.UserId == parsedUserId)
            .OrderBy(p => p.Category)
            .ThenBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);

        var grouped = products.GroupBy(p => p.Category);

        var sb = new StringBuilder();
        sb.AppendLine($"📋 Total products: {products.Count}");
        sb.AppendLine();

        foreach (var group in grouped)
        {
            var categoryNameEn = CategoryHelper.GetNameEn(group.Key);
            var categoryNameUk = CategoryHelper.GetNameUk(group.Key);
            sb.AppendLine($"## {categoryNameEn} ({categoryNameUk}) — {group.Count()} products");

            foreach (var product in group)
            {
                var defaultTag = product.IsDefault ? "" : " [custom]";
                sb.AppendLine($"  • {product.NameEn} ({product.NameUk}){defaultTag}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_product_by_name"), Description(
        "Search for a baby food product by name (Ukrainian or English). " +
        "The search is case-insensitive and matches partial names. " +
        "Returns product details including category, bilingual names, and whether it's a default product.")]
    public static async Task<string> GetProductByName(
        IApplicationDbContext context,
        [Description("The user ID (GUID) to filter products for.")] string userId,
        [Description("Product name to search for (in English or Ukrainian). Supports partial matching.")] string name,
        CancellationToken cancellationToken)
    {
        var (isValid, parsedUserId, error) = UserIdParser.Parse(userId);
        if (!isValid) return error!;
        var normalizedName = name.ToLowerInvariant();

        var products = await context.Products
            .AsNoTracking()
            .Where(p => (p.UserId == null || p.UserId == parsedUserId) &&
                        (p.NameEn.ToLower().Contains(normalizedName) ||
                         p.NameUk.ToLower().Contains(normalizedName)))
            .OrderBy(p => p.Category)
            .ThenBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);

        if (products.Count == 0)
        {
            return $"No products found matching \"{name}\".";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"🔍 Found {products.Count} product(s) matching \"{name}\":");
        sb.AppendLine();

        foreach (var product in products)
        {
            var categoryNameEn = CategoryHelper.GetNameEn(product.Category);
            sb.AppendLine($"• **{product.NameEn}** ({product.NameUk})");
            sb.AppendLine($"  Category: {categoryNameEn}");
            sb.AppendLine($"  Default: {(product.IsDefault ? "Yes" : "No (custom)")}");
            sb.AppendLine($"  ID: {product.Id}");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    [McpServerTool(Name = "get_products_by_category"), Description(
        "Get all baby food products in a specific category. " +
        "Available categories: Vegetables, Fruits, Dairy, Meat, Grains, NutsSeeds, Fish, Spices, Other.")]
    public static async Task<string> GetProductsByCategory(
        IApplicationDbContext context,
        [Description("The user ID (GUID) to filter products for.")] string userId,
        [Description("Category name (e.g., 'Vegetables', 'Fruits', 'Dairy', 'Meat', 'Grains', 'NutsSeeds', 'Fish', 'Spices', 'Other')")] string category,
        CancellationToken cancellationToken)
    {
        var (isValid, parsedUserId, error) = UserIdParser.Parse(userId);
        if (!isValid) return error!;

        if (!Enum.TryParse<ProductCategory>(category, ignoreCase: true, out var parsedCategory))
        {
            var validCategories = string.Join(", ", Enum.GetNames<ProductCategory>());
            return $"Invalid category \"{category}\". Valid categories are: {validCategories}";
        }

        var products = await context.Products
            .AsNoTracking()
            .Where(p => p.Category == parsedCategory && (p.UserId == null || p.UserId == parsedUserId))
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);

        var categoryNameEn = CategoryHelper.GetNameEn(parsedCategory);
        var categoryNameUk = CategoryHelper.GetNameUk(parsedCategory);

        var sb = new StringBuilder();
        sb.AppendLine($"## {categoryNameEn} ({categoryNameUk}) — {products.Count} products");
        sb.AppendLine();

        foreach (var product in products)
        {
            var defaultTag = product.IsDefault ? "" : " [custom]";
            sb.AppendLine($"• {product.NameEn} ({product.NameUk}){defaultTag}");
        }

        return sb.ToString();
    }
}
