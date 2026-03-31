namespace BabyFoodChecklist.Application.Common.Helpers;

public static class CategoryHelper
{
    private static readonly Dictionary<ProductCategory, (string Uk, string En)> CategoryNames = new()
    {
        [ProductCategory.Vegetables] = ("Овочі та зелень", "Vegetables & Greens"),
        [ProductCategory.Fruits] = ("Фрукти", "Fruits"),
        [ProductCategory.Dairy] = ("Молочні продукти", "Dairy Products"),
        [ProductCategory.Meat] = ("М'ясо та тваринні продукти", "Meat & Animal Products"),
        [ProductCategory.Grains] = ("Цільнозернові, крупи та бобові", "Whole Grains, Cereals & Legumes"),
        [ProductCategory.NutsSeeds] = ("Горіхи, насіння", "Nuts & Seeds"),
        [ProductCategory.Fish] = ("Риба, молюски, ракоподібні", "Fish, Molluscs & Crustaceans"),
        [ProductCategory.Spices] = ("Спеції", "Spices"),
        [ProductCategory.Other] = ("Інше", "Other"),
    };

    public static string GetNameUk(ProductCategory category) =>
        CategoryNames.TryGetValue(category, out var names) ? names.Uk : category.ToString();

    public static string GetNameEn(ProductCategory category) =>
        CategoryNames.TryGetValue(category, out var names) ? names.En : category.ToString();
}
