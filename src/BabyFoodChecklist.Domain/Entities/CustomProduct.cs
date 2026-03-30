using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Domain.Entities;

/// <summary>
/// Represents a custom (user-added) product that is not part of the default seed list.
/// </summary>
public class CustomProduct
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the Ukrainian name (optional).</summary>
    public string? NameUk { get; set; }

    /// <summary>Gets or sets the English name (optional).</summary>
    public string? NameEn { get; set; }

    /// <summary>Gets or sets the product category.</summary>
    public ProductCategory Category { get; set; }

    /// <summary>Gets or sets the creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
