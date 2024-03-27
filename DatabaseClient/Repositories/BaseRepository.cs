using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public abstract class BaseRepository<TEntity>(DatabaseContextFactory factory)
    : IRepository<TEntity> where TEntity : class, IEntity
{
    protected DatabaseContextFactory Factory { get; } = factory;

    public virtual async Task<TEntity> GetByIdAsync(int id)
    {
        await using var context = Factory.Create();
        return await context.Set<TEntity>()
            .Where(m => m.Id == id)
            .SingleOrDefaultAsync();
    }

    public virtual async Task<ICollection<TEntity>> GetAllAsync()
    {
        await using var context = Factory.Create();
        return await context.Set<TEntity>().ToArrayAsync();
    }

    public abstract Task UpdateAsync(TEntity entity);

    public virtual async Task RemoveAsync(TEntity entity)
    {
        if (entity == null)
        {
            return;
        }

        await using var context = Factory.Create();
        await context.Set<TEntity>().Where(m => m == entity).ExecuteDeleteAsync();
    }
}