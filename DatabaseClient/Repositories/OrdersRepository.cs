using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class OrdersRepository(DatabaseContextFactory factory) : BaseRepository<Order>(factory)
{
    public async Task<ICollection<Order>> GetOrdersForClientAsync(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        await using var context = Factory.Create();
        return await context.Orders.Where(m => m.ClientId == client.Id).ToListAsync();
    }

    public async Task<ICollection<Order>> GetOrdersForBookAsync(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = Factory.Create();
        return await context.OrdersToBooks.Where(m => m.BookId == book.Id).Select(m => m.Order).ToListAsync();
    }

    public async Task<Order> AddOrderAsync(Client client, IEnumerable<OrdersToBook> booksToOrder)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(booksToOrder);

        var books = booksToOrder.Where(m => m is not null).ToList();
        if (books.Count == 0)
        {
            throw new ArgumentException("Books to order are empty");
        }

        var order = new Order
        {
            ClientId = client.Id,
            OrdersToBooks = books
        };

        await using var context = Factory.Create();
        var entity = await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task<int> GetOrderTotalPrice(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        await using var context = Factory.Create();
        return await context.OrdersToBooks
            .Where(m => m.OrderId == order.Id)
            .SumAsync(m => m.Count * m.Book.Price);
    }

    public override Task UpdateAsync(Order entity)
    {
        throw new NotSupportedException();
    }

    public override Task RemoveAsync(Order entity)
    {
        throw new NotSupportedException();
    }
}