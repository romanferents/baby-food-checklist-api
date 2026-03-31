using BabyFoodChecklist.Domain.Common;
using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Domain.Entities;

/// <summary>
/// Represents a user's entry for a specific product (tracking tried status, rating, notes).
/// </summary>
public class UserProductEntry : BaseEntity
{
    /// <summary>Gets or sets the product identifier this entry is linked to.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Gets or sets a value indicating whether the baby has tried this product.</summary>
    public bool Tried { get; set; }

    /// <summary>Gets or sets the date when the baby first tried this product.</summary>
    public DateTime? FirstTriedAt { get; set; }

    /// <summary>Gets or sets the baby's reaction rating.</summary>
    public FoodRating? Rating { get; set; }

    /// <summary>Gets or sets a note about the baby's reaction.</summary>
    public string? ReactionNote { get; set; }

    /// <summary>Gets or sets additional notes.</summary>
    public string? Notes { get; set; }

    /// <summary>Gets or sets a value indicating whether this product is a favourite.</summary>
    public bool IsFavorite { get; set; }

    /// <summary>Navigation property for the associated product.</summary>
    public virtual Product Product { get; set; } = null!;
}
