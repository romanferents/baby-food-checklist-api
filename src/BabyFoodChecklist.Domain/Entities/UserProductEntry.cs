using BabyFoodChecklist.Domain.Common;
using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Domain.Entities;

public class UserProductEntry : BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public bool Tried { get; set; }
    public DateTimeOffset? FirstTriedAt { get; set; }
    public FoodRating? Rating { get; set; }
    public string? ReactionNote { get; set; }
    public string? Notes { get; set; }
    public bool IsFavorite { get; set; }
}
