using BabyFoodChecklist.Domain.Common;
using BabyFoodChecklist.Infrastructure.Data;

namespace BabyFoodChecklist.Infrastructure.Repositories;

public class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T>
    where T : BaseEntity
{
    protected readonly ApplicationDbContext Context = context;

    public IQueryable<T> Query() => Context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await Context.Set<T>().FindAsync(new object?[] { id }, cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _ = await Context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public void Update(T entity) => Context.Set<T>().Update(entity);

    public void Remove(T entity) => Context.Set<T>().Remove(entity);
}
