﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class OrdersRepository(DbContextFactory factory, IMapper mapper) : IOrdersRepository
{
    public async Task<ICollection<Order>> GetAllAsync()
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<DbOrder>(
                $"""
                 select o.Id as OrderId, o.ClientId, o.CreatedAt,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Orders o
                 join Clients c on c.Id = o.ClientId
                 """)
            .ProjectTo<Order>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Order> GetByIdAsync(int id)
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<DbOrder>(
                $"""
                 select o.Id as OrderId, o.ClientId, o.CreatedAt,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Orders o
                 join Clients c on c.Id = o.ClientId
                 where o.Id = {id}
                 """)
            .ProjectTo<Order>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<Order>> GetOrdersForClientAsync(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<DbOrder>(
                $"""
                 select o.Id as OrderId, o.ClientId, o.CreatedAt,
                        c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted as IsClientDeleted
                 from Orders o
                 join Clients c on c.Id = o.ClientId
                 where c.Id = {client.Id}
                 """)
            .ProjectTo<Order>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ICollection<Order>> GetOrdersForBookAsync(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = factory.Create();
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
            .ProjectTo<Order>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    // bsbd_verify_order verifies that there are enough books for the order,
    // and then subtracts the number of books in the order from the total number
    public async Task<Order> AddOrderAsync(Client client, IEnumerable<OrderBook> booksToOrder)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(booksToOrder);

        var books = booksToOrder.Where(m => m is not null).ToList();
        if (books.Count == 0)
        {
            throw new ArgumentException("Cannot orders empty list of books");
        }

        await using var context = factory.Create();

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
        var order = await context.Database
            .SqlQueryRaw<DbOrder>(query, args)
            .ToListAsync();

        return mapper.Map<Order>(order.SingleOrDefault());
    }

    public async Task<int> GetOrderTotalPrice(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        await using var context = factory.Create();
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

    // bsbd_prevent_orders_change forbids update and delete
    public Task UpdateAsync(Order entity)
    {
        throw new NotSupportedException("Cannot update order");
    }

    public Task RemoveAsync(Order entity)
    {
        throw new NotSupportedException("Cannot delete order");
    }
}