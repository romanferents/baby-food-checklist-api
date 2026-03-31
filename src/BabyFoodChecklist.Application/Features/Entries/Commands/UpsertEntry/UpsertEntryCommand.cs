using BabyFoodChecklist.Application.DTOs;

namespace BabyFoodChecklist.Application.Features.Entries.Commands.UpsertEntry;

public record UpsertEntryCommand : IRequest<UserProductEntryDto>
{
    public Guid ProductId { get; init; }
    public bool Tried { get; init; }
    public DateTimeOffset? FirstTriedAt { get; init; }
    public FoodRating? Rating { get; init; }
    public string? ReactionNote { get; init; }
    public string? Notes { get; init; }
    public bool IsFavorite { get; init; }
}
