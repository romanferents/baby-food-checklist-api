namespace BabyFoodChecklist.Application.DTOs;

/// <summary>
/// Data Transfer Object for overall and per-category progress statistics.
/// </summary>
public class StatisticsDto
{
    /// <summary>Gets or sets the total number of products in the list.</summary>
    public int TotalProducts { get; set; }

    /// <summary>Gets or sets the number of products the baby has tried.</summary>
    public int TriedCount { get; set; }

    /// <summary>Gets or sets the overall percentage of products tried (0–100).</summary>
    public double PercentageTried { get; set; }

    /// <summary>Gets or sets the number of favourite products.</summary>
    public int FavoritesCount { get; set; }

    /// <summary>Gets or sets the breakdown by category.</summary>
    public IEnumerable<CategoryStatisticsDto> ByCategory { get; set; } = Enumerable.Empty<CategoryStatisticsDto>();
}
