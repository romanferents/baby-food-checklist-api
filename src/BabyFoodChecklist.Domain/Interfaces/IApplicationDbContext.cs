using BabyFoodChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Domain.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<UserProductEntry> UserProductEntries { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
