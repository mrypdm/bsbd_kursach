using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public abstract class BaseRepository<TEntity> where TEntity: class, IEntity
{
    public virtual async Task<TEntity> GetById(int id)
    {
        var context = DatabaseContext.Instance;
        return await context.Set<TEntity>()
            .Where(m => m.Id.Equals(id))
            .SingleOrDefaultAsync()
            .ConfigureAwait(false);
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        var context = DatabaseContext.Instance;
        context.Set<TEntity>().Update(entity);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public virtual async Task RemoveAsync(TEntity entity)
    {
        var context = DatabaseContext.Instance;
        context.Set<TEntity>().Remove(entity);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}