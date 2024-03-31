using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IOrderBooksRepository : IRepository<OrdersToBook>
{
    public Task<ICollection<OrdersToBook>> GetBooksForOrderAsync(Order order);
}