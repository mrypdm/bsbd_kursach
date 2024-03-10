using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class OrdersRepository
{
    public async Task<ICollection<Order>> GetOrdersForClientAsync(Client client)
    {
        var context = DatabaseContextFactory.Context;
        return await context.Orders.Where(m => m.ClientId == client.Id).ToListAsync().ConfigureAwait(false);
    }

    public async Task<ICollection<Order>> GetOrdersForBookAsync(Book book)
    {
        var context = DatabaseContextFactory.Context;
        return await context.OrdersToBooks.Where(m => m.BookId == book.Id).Select(m => m.Order).ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<Order> AddOrderAsync(Client client, IEnumerable<OrdersToBook> books)
    {
        var order = new Order
        {
            ClientId = client.Id,
            OrdersToBooks = books.ToList()
        };

        var context = DatabaseContextFactory.Context;
        var entity = await context.Orders.AddAsync(order).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity.Entity;
    }
}