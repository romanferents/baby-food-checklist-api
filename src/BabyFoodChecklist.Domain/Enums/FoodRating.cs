namespace BabyFoodChecklist.Domain.Enums;

/// <summary>
/// Rating of a baby's reaction to the food.
/// </summary>
public enum FoodRating
{
    /// <summary>Baby liked the food.</summary>
    Liked = 1,

    /// <summary>Baby had a neutral reaction.</summary>
    Neutral = 2,

    /// <summary>Baby disliked the food.</summary>
    Disliked = 3
}
