using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IOrdersRepository : IRepository<Order>
{
    Task<ICollection<Order>> GetOrdersForClientAsync(Client client);

    Task<ICollection<Order>> GetOrdersForBookAsync(Book book);

    Task<Order> AddOrderAsync(Client client, IEnumerable<OrdersToBook> booksToOrder);

    Task<int> GetOrderTotalPrice(Order order);
}