using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
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
            .ThenInclude(m => m.Book)
            .ToListAsync();
    }

    public override async Task<Order> GetByIdAsync(int id)
    {
        await using var context = Factory.Create();
        return await context.Orders
            .Where(m => m.Id == id)
            .Include(m => m.Client)
            .Include(m => m.OrdersToBooks)
            .ThenInclude(m => m.Book)
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
            .ThenInclude(m => m.Book)
            .ToListAsync();
    }

    public async Task<ICollection<Order>> GetOrdersForBookAsync(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = Factory.Create();
        return await context.Orders
            .Where(m => m.OrdersToBooks.Any(otb => otb.BookId == book.Id))
            .Include(m => m.Client)
            .Include(m => m.OrdersToBooks)
            .ThenInclude(m => m.Book)
            .ToListAsync();
    }

    // After insert trigger bsbd_verify_order verifies, that there are enough books for the order,
    // and then subtracts the number of books in the order from the total number
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

    // Update are forbidden with trigger bsbd_prevent_update_orders
    public override Task UpdateAsync(Order entity)
    {
        throw new NotSupportedException("Cannot update order");
    }
}