using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Application.DTOs;

/// <summary>
/// Statistics for a single product category.
/// </summary>
public class CategoryStatisticsDto
{
    /// <summary>Gets or sets the category.</summary>
    public ProductCategory Category { get; set; }

    /// <summary>Gets or sets the English category name.</summary>
    public string CategoryNameEn { get; set; } = string.Empty;

    /// <summary>Gets or sets the Ukrainian category name.</summary>
    public string CategoryNameUk { get; set; } = string.Empty;

    /// <summary>Gets or sets the total number of products in the category.</summary>
    public int Total { get; set; }

    /// <summary>Gets or sets the number of products tried in the category.</summary>
    public int Tried { get; set; }

    /// <summary>Gets or sets the percentage tried in the category (0–100).</summary>
    public double Percentage { get; set; }
}
