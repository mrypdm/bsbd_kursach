using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class OrdersRepository : BaseRepository<Order, long>
{
    public async Task<ICollection<Order>> GetOrdersForClientAsync(Client client)
    {
        var context = DatabaseContext.Instance;
        return await context.Orders.Where(m => m.ClientId == client.Id).ToListAsync().ConfigureAwait(false);
    }

    public async Task<ICollection<Order>> GetOrdersForBookAsync(Book book)
    {
        var context = DatabaseContext.Instance;
        return await context.OrdersToBooks.Where(m => m.BookId == book.Id).Select(m => m.Order).ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<Order> AddOrderAsync(Client client, IEnumerable<OrdersToBook> booksToOrder)
    {
        var books = booksToOrder.ToList();

        var order = new Order
        {
            ClientId = client.Id,
            OrdersToBooks = books
        };

        var context = DatabaseContext.Instance;

        foreach (var book in books)
        {
            var currentBook = await context.Books
                .Where(m => m.Id == book.BookId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (currentBook == null)
            {
                throw new InvalidOperationException($"Book with id={book.BookId} does not exist");
            }

            if (currentBook.Count < book.Count)
            {
                throw new InvalidOperationException("Not enough books to order");
            }

            currentBook.Count -= book.Count;
            context.Books.Update(currentBook);
        }


        var entity = await context.Orders.AddAsync(order).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity.Entity;
    }

    public async Task<int> GetOrderTotalPrice(Order order)
    {
        var context = DatabaseContext.Instance;
        return await context.OrdersToBooks
            .Where(m => m.OrderId == order.Id)
            .SumAsync(m => m.Count * m.Book.Price)
            .ConfigureAwait(false);
    }

    public async Task<ICollection<Order>> GetUnpaidOrdersAsync()
    {
        var context = DatabaseContext.Instance;
        return await context.Orders
            .Where(m => m.IsPaid == false)
            .ToListAsync()
            .ConfigureAwait(false);
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