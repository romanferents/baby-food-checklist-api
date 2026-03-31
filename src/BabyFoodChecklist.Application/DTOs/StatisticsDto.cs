namespace BabyFoodChecklist.Application.DTOs;

public record StatisticsDto
{
    public int TotalProducts { get; init; }
    public int TriedProducts { get; init; }
    public double ProgressPercentage { get; init; }
    public IReadOnlyList<CategoryStatisticsDto> CategoryBreakdown { get; init; } = Array.Empty<CategoryStatisticsDto>();
}
