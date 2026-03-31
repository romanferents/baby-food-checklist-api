namespace BabyFoodChecklist.Application.DTOs;

public record CategoryStatisticsDto
{
    public ProductCategory Category { get; init; }
    public string CategoryNameUk { get; init; } = string.Empty;
    public string CategoryNameEn { get; init; } = string.Empty;
    public int TotalProducts { get; init; }
    public int TriedProducts { get; init; }
    public double ProgressPercentage { get; init; }
}
