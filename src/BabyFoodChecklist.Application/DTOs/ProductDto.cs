namespace BabyFoodChecklist.Application.DTOs;

public record ProductDto
{
    public Guid Id { get; init; }
    public string NameUk { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
    public ProductCategory Category { get; init; }
    public string CategoryNameUk { get; init; } = string.Empty;
    public string CategoryNameEn { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
    public int SortOrder { get; init; }
}
