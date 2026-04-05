using BabyFoodChecklist.Domain.Common;

namespace BabyFoodChecklist.Domain.Entities;

public class ApplicationUser : BaseAuditableEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<UserProductEntry> Entries { get; set; } = new List<UserProductEntry>();
    public ICollection<Product> CustomProducts { get; set; } = new List<Product>();
}
