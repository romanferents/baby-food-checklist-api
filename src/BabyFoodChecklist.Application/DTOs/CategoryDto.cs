using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Application.DTOs;

/// <summary>
/// Data Transfer Object representing a food category with localised names.
/// </summary>
public class CategoryDto
{
    /// <summary>Gets or sets the category enum value.</summary>
    public ProductCategory Value { get; set; }

    /// <summary>Gets or sets the English category name.</summary>
    public string NameEn { get; set; } = string.Empty;

    /// <summary>Gets or sets the Ukrainian category name.</summary>
    public string NameUk { get; set; } = string.Empty;
}
