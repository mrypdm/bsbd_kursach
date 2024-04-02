using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class OrderBooksRepository(DbContextFactory factory, IMapper mapper) : IOrderBooksRepository
{
    public async Task<OrderBook> GetByIdAsync(int orderId, int bookId)
    {
        await using var context = factory.Create();

        return await context.Database
            .SqlQuery<DbOrderBook>(
                $"""
                 select otb.BookId, otb.OrderId, otb.Count as OrderedCount,
                        b.Title as BookTitle, b.Author as BookAuthor, b.ReleaseDate as BookReleaseDate,
                        b.IsDeleted as IsBookDeleted, b.Count as TotalCount, b.Price as BookPrice
                 from OrdersToBooks otb
                 join Books b on otb.BookId = b.Id
                 where OrderId = {orderId} and BookId = {bookId}
                 """)
            .ProjectTo<OrderBook>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<OrderBook>> GetBooksForOrderAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        await using var context = factory.Create();

        return await context.Database
            .SqlQuery<DbOrderBook>(
                $"""
                 select otb.BookId, otb.OrderId, otb.Count as OrderedCount,
                        b.Title as BookTitle, b.Author as BookAuthor, b.ReleaseDate as BookReleaseDate,
                        b.IsDeleted as IsBookDeleted, b.Count as TotalCount, b.Price as BookPrice
                 from OrdersToBooks otb
                 join Books b on otb.BookId = b.Id
                 where OrderId = {order.Id}
                 """)
            .ProjectTo<OrderBook>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public Task<OrderBook> GetByIdAsync(int id)
    {
        throw new NotSupportedException("Use GetByIdAsync(int orderId, int bookId)");
    }

    public Task<ICollection<OrderBook>> GetAllAsync()
    {
        throw new NotSupportedException();
    }

    public Task UpdateAsync(OrderBook entity)
    {
        throw new NotSupportedException("Cannot update existing order");
    }

    public Task RemoveAsync(OrderBook entity)
    {
        throw new NotSupportedException("Cannot delete existing order");
    }
}