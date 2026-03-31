using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Application.DTOs;

/// <summary>
/// Data Transfer Object representing a user product entry.
/// </summary>
public class UserProductEntryDto
{
    /// <summary>Gets or sets the entry identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the product identifier.</summary>
    public Guid ProductId { get; set; }

    /// <summary>Gets or sets a value indicating whether the baby has tried this product.</summary>
    public bool Tried { get; set; }

    /// <summary>Gets or sets the date when the baby first tried the product.</summary>
    public DateTime? FirstTriedAt { get; set; }

    /// <summary>Gets or sets the baby's rating for the product.</summary>
    public FoodRating? Rating { get; set; }

    /// <summary>Gets or sets a note about the baby's reaction.</summary>
    public string? ReactionNote { get; set; }

    /// <summary>Gets or sets additional notes.</summary>
    public string? Notes { get; set; }

    /// <summary>Gets or sets a value indicating whether this is a favourite product.</summary>
    public bool IsFavorite { get; set; }

    /// <summary>Gets or sets the associated product details.</summary>
    public ProductDto? Product { get; set; }

    /// <summary>Gets or sets the creation timestamp.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Gets or sets the last update timestamp.</summary>
    public DateTime UpdatedAt { get; set; }
}
