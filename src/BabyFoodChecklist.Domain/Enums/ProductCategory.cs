namespace BabyFoodChecklist.Domain.Enums;

/// <summary>
/// Represents the food category of a product.
/// </summary>
public enum ProductCategory
{
    /// <summary>Овочі та зелень / Vegetables &amp; Greens</summary>
    Vegetables = 1,

    /// <summary>Фрукти / Fruits</summary>
    Fruits = 2,

    /// <summary>Цільнозернові, крупи та бобові / Grains &amp; Legumes</summary>
    Grains = 3,

    /// <summary>М'ясо та тваринні продукти / Meat &amp; Animal Products</summary>
    Meat = 4,

    /// <summary>Риба, молюски, ракоподібні / Fish &amp; Seafood</summary>
    Fish = 5,

    /// <summary>Молочні продукти / Dairy</summary>
    Dairy = 6,

    /// <summary>Горіхи, насіння / Nuts &amp; Seeds</summary>
    NutsSeeds = 7,

    /// <summary>Спеції / Spices</summary>
    Spices = 8,

    /// <summary>Інше / Other</summary>
    Other = 9
}
