using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IRepository<TEntity> where TEntity : IEntity
{
    Task<TEntity> GetByIdAsync(int id);

    Task<ICollection<TEntity>> GetAllAsync();

    Task UpdateAsync(TEntity entity);

    Task RemoveAsync(TEntity entity);
}