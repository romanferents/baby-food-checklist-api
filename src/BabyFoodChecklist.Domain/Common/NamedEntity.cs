namespace BabyFoodChecklist.Domain.Common;

public abstract class NamedEntity : BaseAuditableEntity
{
    public string NameUk { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
}
