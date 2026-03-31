namespace BabyFoodChecklist.Application.DTOs;

public record UserProductEntryDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductNameUk { get; init; } = string.Empty;
    public string ProductNameEn { get; init; } = string.Empty;
    public bool Tried { get; init; }
    public DateTimeOffset? FirstTriedAt { get; init; }
    public FoodRating? Rating { get; init; }
    public string? ReactionNote { get; init; }
    public string? Notes { get; init; }
    public bool IsFavorite { get; init; }
}
