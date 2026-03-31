using BabyFoodChecklist.Domain.Events;
using BabyFoodChecklist.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BabyFoodChecklist.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEnumerable<ISaveChangesInterceptor> interceptors)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<UserProductEntry> UserProductEntries => Set<UserProductEntry>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(interceptors);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<BaseEvent>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
