namespace BabyFoodChecklist.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern — coordinates changes across repositories and persists them atomically.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Gets the product repository.</summary>
    IProductRepository Products { get; }

    /// <summary>Gets the user product entry repository.</summary>
    IUserProductEntryRepository Entries { get; }

    /// <summary>Saves all pending changes to the data store.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
