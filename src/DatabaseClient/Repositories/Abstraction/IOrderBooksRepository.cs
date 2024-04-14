using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IOrderBooksRepository : IRepository<OrderBook>
{
    public Task<ICollection<OrderBook>> GetBooksForOrderAsync(Order order);
}