using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseClient.Repositories.Abstraction;

public interface IRepository<TEntity>
{
    Task<TEntity> GetByIdAsync(int id);

    Task<ICollection<TEntity>> GetAllAsync();

    Task UpdateAsync(TEntity entity);

    Task RemoveAsync(TEntity entity);
}