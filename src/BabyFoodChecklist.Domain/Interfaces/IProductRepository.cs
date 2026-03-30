using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Domain.Interfaces;

/// <summary>
/// Repository interface for <see cref="Product"/> entities.
/// </summary>
public interface IProductRepository
{
    /// <summary>Gets a product by its unique identifier.</summary>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets a paged list of products with optional filtering.</summary>
    Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        ProductCategory? category = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new product.</summary>
    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing product.</summary>
    void Update(Product product);

    /// <summary>Removes a product.</summary>
    void Remove(Product product);
}
