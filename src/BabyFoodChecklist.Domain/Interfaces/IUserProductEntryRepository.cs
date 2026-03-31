using BabyFoodChecklist.Domain.Entities;
using BabyFoodChecklist.Domain.Enums;

namespace BabyFoodChecklist.Domain.Interfaces;

/// <summary>
/// Repository interface for <see cref="UserProductEntry"/> entities.
/// </summary>
public interface IUserProductEntryRepository
{
    /// <summary>Gets an entry by its unique identifier.</summary>
    Task<UserProductEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets an entry for a specific product.</summary>
    Task<UserProductEntry?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    /// <summary>Gets a paged list of entries with optional filtering.</summary>
    Task<(IEnumerable<UserProductEntry> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        bool? tried = null,
        bool? isFavorite = null,
        ProductCategory? category = null,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new entry.</summary>
    Task AddAsync(UserProductEntry entry, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing entry.</summary>
    void Update(UserProductEntry entry);

    /// <summary>Removes an entry.</summary>
    void Remove(UserProductEntry entry);
}
