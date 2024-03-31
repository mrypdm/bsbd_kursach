using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public abstract class BaseRepository<TEntity>(DatabaseContextFactory factory)
    : IRepository<TEntity> where TEntity : class
{
    protected DatabaseContextFactory Factory { get; } = factory;

    public abstract Task<TEntity> GetByIdAsync(int id);

    public virtual async Task<ICollection<TEntity>> GetAllAsync()
    {
        await using var context = Factory.Create();
        return await context.Set<TEntity>().ToArrayAsync();
    }

    public abstract Task UpdateAsync(TEntity entity);

    public abstract Task RemoveAsync(TEntity entity);
}