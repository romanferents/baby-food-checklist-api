using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;
using BabyFoodChecklist.Domain.Interfaces;
using BabyFoodChecklist.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BabyFoodChecklist.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IProductRepository"/>.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>Initializes a new instance of <see cref="ProductRepository"/>.</summary>
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Products.FindAsync(new object[] { id }, cancellationToken);

    /// <inheritdoc />
    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        ProductCategory? category = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();

        if (category.HasValue)
            query = query.Where(p => p.Category == category.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lower = searchTerm.ToLower();
            query = query.Where(p =>
                p.NameEn.ToLower().Contains(lower) ||
                p.NameUk.ToLower().Contains(lower));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.NameEn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => await _context.Products.AddAsync(product, cancellationToken);

    /// <inheritdoc />
    public void Update(Product product)
        => _context.Products.Update(product);

    /// <inheritdoc />
    public void Remove(Product product)
        => _context.Products.Remove(product);
}
