namespace BabyFoodChecklist.Application.DTOs;

public record CategoryDto
{
    public int Value { get; init; }
    public string NameUk { get; init; } = string.Empty;
    public string NameEn { get; init; } = string.Empty;
}
