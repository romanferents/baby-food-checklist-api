using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using BabyFoodChecklist.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserProductEntryRepository"/>.
/// </summary>
public class UserProductEntryRepository : IUserProductEntryRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of <see cref="UserProductEntryRepository"/>.</summary>
    public UserProductEntryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<UserProductEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.UserProductEntries
            .Include(e => e.Product)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    /// <inheritdoc />
    public async Task<UserProductEntry?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        => await _context.UserProductEntries
            .Include(e => e.Product)
            .FirstOrDefaultAsync(e => e.ProductId == productId, cancellationToken);

    /// <inheritdoc />
    public async Task<(IEnumerable<UserProductEntry> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        bool? tried = null,
        bool? isFavorite = null,
        ProductCategory? category = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.UserProductEntries
            .Include(e => e.Product)
            .AsQueryable();

        if (tried.HasValue)
            query = query.Where(e => e.Tried == tried.Value);

        if (isFavorite.HasValue)
            query = query.Where(e => e.IsFavorite == isFavorite.Value);

        if (category.HasValue)
            query = query.Where(e => e.Product.Category == category.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task AddAsync(UserProductEntry entry, CancellationToken cancellationToken = default)
        => await _context.UserProductEntries.AddAsync(entry, cancellationToken);

    /// <inheritdoc />
    public void Update(UserProductEntry entry)
        => _context.UserProductEntries.Update(entry);

    /// <inheritdoc />
    public void Remove(UserProductEntry entry)
        => _context.UserProductEntries.Remove(entry);
}
