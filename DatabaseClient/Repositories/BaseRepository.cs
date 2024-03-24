using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public abstract class BaseRepository<TEntity>(DatabaseContextFactory factory) where TEntity : class, IEntity
{
    protected DatabaseContextFactory Factory { get; } = factory;

    public virtual async Task<TEntity> GetById(int id)
    {
        await using var context = Factory.Create();
        return await context.Set<TEntity>()
            .Where(m => m.Id.Equals(id))
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<TEntity>> GetAllAsync()
    {
        await using var context = Factory.Create();
        return await context.Set<TEntity>().ToArrayAsync();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = Factory.Create();
        context.Set<TEntity>().Update(entity);
        await context.SaveChangesAsync();
    }

    public virtual async Task RemoveAsync(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = Factory.Create();
        await context.Set<TEntity>().Where(m => m == entity).ExecuteDeleteAsync();
    }
}