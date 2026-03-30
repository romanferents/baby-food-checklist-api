using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Application.Common.Helpers;

/// <summary>
/// Provides localised display names for <see cref="ProductCategory"/> values.
/// </summary>
public static class CategoryHelper
{
    private static readonly Dictionary<ProductCategory, (string En, string Uk)> Names = new()
    {
        { ProductCategory.Vegetables, ("Vegetables & Greens", "Овочі та зелень") },
        { ProductCategory.Fruits,     ("Fruits",             "Фрукти") },
        { ProductCategory.Grains,     ("Grains & Legumes",   "Цільнозернові, крупи та бобові") },
        { ProductCategory.Meat,       ("Meat & Animal Products", "М'ясо та тваринні продукти") },
        { ProductCategory.Fish,       ("Fish & Seafood",     "Риба, молюски, ракоподібні") },
        { ProductCategory.Dairy,      ("Dairy",              "Молочні продукти") },
        { ProductCategory.NutsSeeds,  ("Nuts & Seeds",       "Горіхи, насіння") },
        { ProductCategory.Spices,     ("Spices",             "Спеції") },
        { ProductCategory.Other,      ("Other",              "Інше") },
    };

    /// <summary>Returns the English display name for a category.</summary>
    public static string GetNameEn(ProductCategory category) =>
        Names.TryGetValue(category, out var n) ? n.En : category.ToString();

    /// <summary>Returns the Ukrainian display name for a category.</summary>
    public static string GetNameUk(ProductCategory category) =>
        Names.TryGetValue(category, out var n) ? n.Uk : category.ToString();
}
