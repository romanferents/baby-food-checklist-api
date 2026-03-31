using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Application.DTOs;

/// <summary>
/// Data Transfer Object representing a product.
/// </summary>
public class ProductDto
{
    /// <summary>Gets or sets the product identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the Ukrainian name.</summary>
    public string NameUk { get; set; } = string.Empty;

    /// <summary>Gets or sets the English name.</summary>
    public string NameEn { get; set; } = string.Empty;

    /// <summary>Gets or sets the product category.</summary>
    public ProductCategory Category { get; set; }

    /// <summary>Gets or sets the category name in English.</summary>
    public string CategoryNameEn { get; set; } = string.Empty;

    /// <summary>Gets or sets the category name in Ukrainian.</summary>
    public string CategoryNameUk { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether this is a default product.</summary>
    public bool IsDefault { get; set; }

    /// <summary>Gets or sets the sort order.</summary>
    public int SortOrder { get; set; }

    /// <summary>Gets or sets the creation timestamp.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the last update timestamp.</summary>
    public DateTime UpdatedAt { get; set; }
}
