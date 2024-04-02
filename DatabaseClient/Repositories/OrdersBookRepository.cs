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

public class OrdersBookRepository(DbContextFactory factory, IMapper mapper) : IOrderBooksRepository
{
    public async Task<OrdersToBook> GetByIdAsync(int orderId, int bookId)
    {
        await using var context = factory.Create();

        return await context.Database
            .SqlQuery<DbOrderToBook>(
                $"""
                 select otb.BookId, otb.OrderId, otb.Count as OrderedCount,
                        b.Title as BookTitle, b.Author as BookAuthor, b.ReleaseDate as BookReleaseDate,
                        b.IsDeleted as IsBookDeleted, b.Count as TotalCount, b.Price as BookPrice
                 from OrdersToBooks otb
                 join Books b on otb.BookId = b.Id
                 where OrderId = {orderId} and BookId = {bookId}
                 """)
            .ProjectTo<OrdersToBook>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<OrdersToBook>> GetBooksForOrderAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        await using var context = factory.Create();

        return await context.Database
            .SqlQuery<DbOrderToBook>(
                $"""
                 select otb.BookId, otb.OrderId, otb.Count as OrderedCount,
                        b.Title as BookTitle, b.Author as BookAuthor, b.ReleaseDate as BookReleaseDate,
                        b.IsDeleted as IsBookDeleted, b.Count as TotalCount, b.Price as BookPrice
                 from OrdersToBooks otb
                 join Books b on otb.BookId = b.Id
                 where OrderId = {order.Id}
                 """)
            .ProjectTo<OrdersToBook>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public Task<OrdersToBook> GetByIdAsync(int id)
    {
        throw new NotSupportedException("Use GetByIdAsync(int orderId, int bookId)");
    }

    public Task<ICollection<OrdersToBook>> GetAllAsync()
    {
        throw new NotSupportedException();
    }

    public Task UpdateAsync(OrdersToBook entity)
    {
        throw new NotSupportedException("Cannot update existing order");
    }

    public Task RemoveAsync(OrdersToBook entity)
    {
        throw new NotSupportedException("Cannot delete existing order");
    }
}