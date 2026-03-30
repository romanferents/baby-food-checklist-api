using BabyFoodChecklist.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BabyFoodChecklist.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for the Baby Food Checklist application.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationDbContext"/>.
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>Gets or sets the products DbSet.</summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>Gets or sets the user product entries DbSet.</summary>
    public DbSet<UserProductEntry> UserProductEntries => Set<UserProductEntry>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
