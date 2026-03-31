using BabyFoodChecklist.Domain.Interfaces;
using BabyFoodChecklist.Infrastructure.Persistence;
using BabyFoodChecklist.Infrastructure.Repositories;

namespace BabyFoodChecklist.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/>.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of <see cref="UnitOfWork"/>.</summary>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Products = new ProductRepository(context);
        Entries = new UserProductEntryRepository(context);
    }

    /// <inheritdoc />
    public IProductRepository Products { get; }

    /// <inheritdoc />
    public IUserProductEntryRepository Entries { get; }

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
