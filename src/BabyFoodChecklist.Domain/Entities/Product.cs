using BabyFoodChecklist.Domain.Common;
using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Domain.Entities;

/// <summary>
/// Represents a food product in the baby checklist.
/// </summary>
public class Product : BaseEntity
{
    /// <summary>Gets or sets the Ukrainian name of the product.</summary>
    public string NameUk { get; set; } = string.Empty;

    /// <summary>Gets or sets the English name of the product.</summary>
    public string NameEn { get; set; } = string.Empty;

    /// <summary>Gets or sets the category of the product.</summary>
    public ProductCategory Category { get; set; }

    /// <summary>Gets or sets a value indicating whether this is a default seeded product.</summary>
    public bool IsDefault { get; set; }

    /// <summary>Gets or sets the display sort order.</summary>
    public int SortOrder { get; set; }

    /// <summary>Navigation property for user entries associated with this product.</summary>
    public virtual ICollection<UserProductEntry> UserProductEntries { get; set; } = new List<UserProductEntry>();
}
