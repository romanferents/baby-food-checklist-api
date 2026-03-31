using BabyFoodChecklist.Domain.Common;
using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Domain.Entities;

public class Product : NamedEntity
{
    public ProductCategory Category { get; set; }
    public bool IsDefault { get; set; }
    public int SortOrder { get; set; }
    public ICollection<UserProductEntry> UserEntries { get; set; } = new List<UserProductEntry>();
}
