using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ClientsRepository(DatabaseContextFactory factory) : IClientsRepository
{
    // Instead of delete, the record is updated by trigger bsbd_mark_user_deleted with the parameters:
    // FirstName = empty,
    // LastName = empty,
    // Phone = 0000000000,
    // IsDeleted = true

    public async Task<Client> GetByIdAsync(int id)
    {
        await using var context = factory.Create();

        // return await context.Clients
        //     .Where(m => m.Id == id && !m.IsDeleted)
        //     .SingleOrDefaultAsync();

        return await context.Database
            .SqlQuery<DbClient>(
                $"""
                 select c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
                 from Clients c
                 where c.IsDeleted = 0 and c.Id = {id}
                 order by c.Id
                 """)
            .SingleOrDefaultAsync<DbClient, Client>();
    }

    public async Task<ICollection<Client>> GetAllAsync()
    {
        await using var context = factory.Create();

        // return await context.Clients
        //     .Where(m => !m.IsDeleted)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbClient>(
                $"""
                 select c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
                 from Clients c
                 where c.IsDeleted = 0
                 order by c.Id
                 """)
            .AsListAsync<DbClient, Client>();
    }

    public async Task<Client> GetClientsByPhoneAsync(string phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        await using var context = factory.Create();

        // return await context.Clients
        //     .Where(m => m.Phone == phone && !m.IsDeleted)
        //     .SingleOrDefaultAsync();

        return await context.Database
            .SqlQuery<DbClient>(
                $"""
                 select c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
                 from Clients c
                 where c.IsDeleted = 0 and c.Phone = {phone}
                 order by c.Id
                 """)
            .SingleOrDefaultAsync<DbClient, Client>();
    }

    public async Task<ICollection<Client>> GetClientsByNameAsync(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Both of names cannot be empty");
        }

        await using var context = factory.Create();

        // var command = context.Clients as IQueryable<Client>;
        //
        // if (!string.IsNullOrWhiteSpace(firstName))
        // {
        //     command = command.Where(m => m.FirstName == firstName);
        // }
        //
        // if (!string.IsNullOrWhiteSpace(lastName))
        // {
        //     command = command.Where(m => m.LastName == lastName);
        // }
        //
        // return await command.ToListAsync();

        var builder = new StringBuilder(
            """
            select c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
            from Clients c
            where c.IsDeleted = 0
            """);

        var sqlParams = new List<SqlParameter>();

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            builder.Append(" and c.FirstName = @firstName");
            sqlParams.Add(new SqlParameter("@firstName", firstName));
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            builder.Append(" and c.LastName = @lastName");
            sqlParams.Add(new SqlParameter("@lastName", lastName));
        }

        builder.Append("\norder by c.Id");

        // ReSharper disable once CoVariantArrayConversion
        return await context.Database
            .SqlQueryRaw<DbClient>(builder.ToString(), sqlParams.ToArray())
            .AsListAsync<DbClient, Client>();
    }

    public async Task<ICollection<Client>> GetClientsByGender(Gender gender)
    {
        await using var context = factory.Create();

        // return await context.Clients
        //     .Where(m => m.Gender == gender && !m.IsDeleted)
        //     .ToArrayAsync();

        return await context.Database
            .SqlQuery<DbClient>(
                $"""
                 select c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
                 from Clients c
                 where c.IsDeleted = 0 and c.Gender = {gender}
                 order by c.Id
                 """)
            .AsListAsync<DbClient, Client>();
    }

    public async Task<Client> AddClientAsync(string firstName, string lastName, string phone, Gender gender)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        await using var context = factory.Create();

        // var client = new Client
        // {
        //     FirstName = firstName,
        //     LastName = lastName,
        //     Phone = phone,
        //     Gender = gender
        // };
        //
        // var entity = await context.Clients.AddAsync(client);
        // await context.SaveChangesAsync();
        // return entity.Entity;

        return await context.Database
            .SqlQuery<DbClient>(
                $"""
                 insert into Clients (FirstName, LastName, Phone, Gender)
                 output inserted.*
                 values ({firstName}, {lastName}, {phone}, {gender})
                 """)
            .SingleOrDefaultAsync<DbClient, Client>();
    }

    public async Task UpdateAsync(Client entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = factory.Create();

        // await context.Clients
        //     .Where(m => m.Id == entity.Id)
        //     .ExecuteUpdateAsync(o => o
        //         .SetProperty(m => m.FirstName, entity.FirstName)
        //         .SetProperty(m => m.LastName, entity.LastName)
        //         .SetProperty(m => m.Phone, entity.Phone)
        //         .SetProperty(m => m.Gender, entity.Gender));

        await context.Database.ExecuteSqlAsync(
            $"""
             update Books
             set FirstName = {entity.FirstName}, LastName = {entity.LastName},
                 Phone = {entity.Phone}, Gender = {entity.Gender}
             where Id = {entity.Id}
             """);
    }

    public async Task RemoveAsync(Client entity)
    {
        if (entity == null)
        {
            return;
        }

        await using var context = factory.Create();
        await context.Database.ExecuteSqlAsync($"delete from Clients where Id == {entity.Id}");
    }

    public async Task<int> RevenueOfClient(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        await using var context = factory.Create();

        // return await context.Orders
        //     .Where(m => m.ClientId == client.Id && !m.Client.IsDeleted)
        //     .SelectMany(m => m.OrdersToBooks)
        //     .SumAsync(m => m.Count * m.Book.Price);

        return await context.Database
            .SqlQuery<int>(
                $"""
                 select coalesce(sum(otb.Count * b.Price), 0) as Value
                 from Orders o
                 join Clients c on o.ClientId = c.Id
                 join OrdersToBooks otb on o.Id = otb.OrderId
                 join Books b on otb.BookId = b.Id
                 where c.IsDeleted = 0 and o.ClientId = {client.Id}
                 """)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<Client>> MostRevenueClients(int topCount = 10)
    {
        await using var context = factory.Create();

        // return await context.Clients
        //     .Where(m => !m.IsDeleted)
        //     .Select(m => new
        //     {
        //         Client = m,
        //         Sum = m.Orders.SelectMany(order
        //                 => order.OrdersToBooks.Select(orb => orb.Count * orb.Book.Price))
        //             .Sum()
        //     })
        //     .OrderByDescending(m => m.Sum)
        //     .Take(topCount)
        //     .Select(m => m.Client)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbClient>(
                $"""
                 select top({topCount}) c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
                 from Clients c
                 where c.IsDeleted = 0
                 order by
                 (
                     select coalesce(sum(orv.Revenue), 0)
                     from Orders o
                     join
                     (
                         select otb.OrderId, otb.Count * b.Price as Revenue
                         from OrdersToBooks otb
                         join Books b on otb.BookId = b.Id
                     ) as orv on orv.OrderId = o.Id
                 ) desc, c.Id
                 """)
            .AsListAsync<DbClient, Client>();
    }
}