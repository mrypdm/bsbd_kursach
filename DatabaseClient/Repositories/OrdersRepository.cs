using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class OrdersRepository(DatabaseContextFactory factory) : BaseRepository<Order>(factory), IOrdersRepository
{
    // Can only delete OrderToBooks entity when deleting whole order

    public override async Task<ICollection<Order>> GetAllAsync()
    {
        await using var context = Factory.Create();
        return await context.Orders
            .Include(m => m.Client)
            .Include(m => m.OrdersToBooks)
            .ToListAsync();
    }

    public override async Task<Order> GetByIdAsync(int id)
    {
        await using var context = Factory.Create();
        return await context.Orders
            .Where(m => m.Id == id)
            .Include(m => m.Client)
            .Include(m => m.OrdersToBooks)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<Order>> GetOrdersForClientAsync(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        await using var context = Factory.Create();
        return await context.Orders
            .Where(m => m.ClientId == client.Id)
            .Include(m => m.Client)
            .Include(m => m.OrdersToBooks)
            .ToListAsync();
    }

    public async Task<ICollection<Order>> GetOrdersForBookAsync(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = Factory.Create();
        return await context.OrdersToBooks
            .Where(m => m.BookId == book.Id)
            .Select(m => m.Order)
            .Include(m => m.Client)
            .Include(m => m.OrdersToBooks)
            .ToListAsync();
    }

    // After insert trigger bsbd_verify_order verifies, that there are enough books for the order,
    // and then subtracts the number of books in the order from the total number
    public async Task<Order> AddOrderAsync(Client client, IEnumerable<OrdersToBook> booksToOrder)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(booksToOrder);

        await using var context = Factory.Create();
        var orderId = await context.Database
            .SqlQuery<int>(
                $"""
                 insert into Orders (ClientId)
                 output inserted.Id
                 values ({client.Id})
                 """)
            .GetInserted();

        foreach (var book in booksToOrder)
        {
            await context.Database.ExecuteSqlAsync(
                $"""
                 insert into OrdersToBooks(OrderId, BookId, Count)
                 values({orderId}, {book.BookId}, {book.Count})
                 """
            );
        }

        return await context.Orders
            .Where(m => m.Id == orderId)
            .Include(m => m.OrdersToBooks)
            .SingleAsync();
    }

    public async Task<int> GetOrderTotalPrice(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        await using var context = Factory.Create();
        return await context.OrdersToBooks
            .Where(m => m.OrderId == order.Id)
            .SumAsync(m => m.Count * m.Book.Price);
    }

    // Update are forbidden with trigger bsbd_prevent_update_orders
    public override Task UpdateAsync(Order entity)
    {
        throw new NotSupportedException("Cannot update order");
    }
}