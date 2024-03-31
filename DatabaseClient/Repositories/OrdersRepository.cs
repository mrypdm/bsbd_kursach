using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class OrdersRepository(DatabaseContextFactory factory) : IOrdersRepository
{
    // Can only delete OrderToBooks entity when deleting whole order

    public async Task<ICollection<Order>> GetAllAsync()
    {
        await using var context = factory.Create();

        // return await context.Orders
        //     .Include(m => m.Client)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbOrder>(
                $"""
                 select o.Id as OrderId, o.ClientId, o.CreatedAt,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Orders o
                 join Clients c on c.Id = o.ClientId
                 """)
            .AsListAsync<DbOrder, Order>();
    }

    public async Task<Order> GetByIdAsync(int id)
    {
        await using var context = factory.Create();

        // return await context.Orders
        //     .Where(m => m.Id == id)
        //     .Include(m => m.Client)
        //     .SingleOrDefaultAsync();

        return await context.Database
            .SqlQuery<DbOrder>(
                $"""
                 select o.Id as OrderId, o.ClientId, o.CreatedAt,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Orders o
                 join Clients c on c.Id = o.ClientId
                 where o.Id == {id}
                 """)
            .SingleOrDefaultAsync<DbOrder, Order>();
    }

    public async Task<ICollection<Order>> GetOrdersForClientAsync(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        await using var context = factory.Create();

        // return await context.Orders
        //     .Where(m => m.ClientId == client.Id)
        //     .Include(m => m.Client)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbOrder>(
                $"""
                 select o.Id as OrderId, o.ClientId, o.CreatedAt,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Orders o
                 join Clients c on c.Id = o.ClientId
                 where c.Id == {client.Id}
                 """)
            .AsListAsync<DbOrder, Order>();
    }

    public async Task<ICollection<Order>> GetOrdersForBookAsync(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = factory.Create();

        // return await context.Orders
        //     .Where(m => m.OrdersToBooks.Any(otb => otb.BookId == book.Id))
        //     .Include(m => m.Client)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbOrder>(
                $"""
                 select o.Id as OrderId, o.ClientId, o.CreatedAt,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Orders o
                 join Clients c on c.Id = o.ClientId
                 where exists(select 1 from OrdersToBooks otb
                                       where o.Id = otb.OrderId and otb.BookId = {book.Id})
                 """)
            .AsListAsync<DbOrder, Order>();
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
            throw new ArgumentException("Cannot orders empty list of books");
        }

        await using var context = factory.Create();

        // var order = new Order
        // {
        //     ClientId = client.Id,
        //     OrdersToBooks = books
        // };
        //
        // var entity = await context.Orders.AddAsync(order);
        // await context.SaveChangesAsync();
        // return entity.Entity;

        var booksStr = string.Join(", ", books.Select((_, i) => $"(@orderId, @book{i}_id, @book{i}_count)"));

        var args = books
            .SelectMany((m, i) => new[]
            {
                new SqlParameter($"@book{i}_id", m.BookId),
                new SqlParameter($"@book{i}_count", m.Count)
            })
            .Append(new SqlParameter("@clientId", client.Id))
            .ToArray();

        var query = $"""
                     declare @orders table(Id int)

                     insert into Orders (ClientId)
                     output inserted.Id into @orders
                     values (@clientId)

                     declare @orderId int
                     set @orderId = (select top(1) Id from @orders)

                     insert into OrdersToBooks (OrderId, BookId, Count)
                     values {booksStr}

                     select o.Id as OrderId, o.ClientId, o.CreatedAt,
                            c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                     from Orders o
                     join Clients c on c.Id = o.ClientId
                     where o.Id = @orderId
                     """;

        // ReSharper disable once CoVariantArrayConversion
        return await context.Database
            .SqlQueryRaw<DbOrder>(query, args)
            .SingleOrDefaultAsync<DbOrder, Order>();
    }

    public async Task<int> GetOrderTotalPrice(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        await using var context = factory.Create();

        // return await context.OrdersToBooks
        //     .Where(m => m.OrderId == order.Id)
        //     .SumAsync(m => m.Count * m.Book.Price);

        return await context.Database
            .SqlQuery<int>(
                $"""
                 select COALESCE(SUM(otb.Count * b.Price), 0) as Value
                 from OrdersToBooks otb
                 join Books b on b.Id = otb.BookId
                 where otb.OrderId = {order.Id}
                 """)
            .SingleOrDefaultAsync();
    }

    // Update are forbidden with trigger bsbd_prevent_update_orders
    public Task UpdateAsync(Order entity)
    {
        throw new NotSupportedException("Cannot update order");
    }

    public Task RemoveAsync(Order entity)
    {
        throw new NotSupportedException("Cannot delete order");
    }
}