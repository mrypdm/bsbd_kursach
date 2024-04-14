using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ClientsRepository(DbContextFactory factory) : IClientsRepository
{
    public async Task<Client> GetByIdAsync(int id)
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<Client>(
                $"""
                 select c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
                 from Clients c
                 where c.IsDeleted = 0 and c.Id = {id}
                 """)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<Client>> GetAllAsync()
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<Client>(
                $"""
                 select c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
                 from Clients c
                 where c.IsDeleted = 0
                 order by c.Id
                 """)
            .ToListAsync();
    }

    public async Task<Client> GetClientsByPhoneAsync(string phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<Client>(
                $"""
                 select c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
                 from Clients c
                 where c.IsDeleted = 0 and c.Phone = {phone}
                 """)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<Client>> GetClientsByNameAsync(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Both of names cannot be empty");
        }

        await using var context = factory.Create();

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
            .SqlQueryRaw<Client>(builder.ToString(), sqlParams.ToArray())
            .ToListAsync();
    }

    public async Task<ICollection<Client>> GetClientsByGender(Gender gender)
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<Client>(
                $"""
                 select c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
                 from Clients c
                 where c.IsDeleted = 0 and c.Gender = {gender}
                 order by c.Id
                 """)
            .ToListAsync();
    }

    public async Task<Client> AddClientAsync(string firstName, string lastName, string phone, Gender gender)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        await using var context = factory.Create();
        var client = await context.Database
            .SqlQuery<Client>(
                $"""
                 insert into Clients (FirstName, LastName, Phone, Gender)
                 output inserted.*
                 values ({firstName}, {lastName}, {phone}, {gender})
                 """)
            .ToListAsync();

        return client.SingleOrDefault();
    }

    public async Task UpdateAsync(Client entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = factory.Create();
        await context.Database.ExecuteSqlAsync(
            $"""
             update Clients
             set FirstName = {entity.FirstName}, LastName = {entity.LastName},
                 Phone = {entity.Phone}, Gender = {entity.Gender}
             where Id = {entity.Id}
             """);
    }

    // bsbd_mark_client_as_deleted prevents deletion and marks client as deleted and clears all data
    public async Task RemoveAsync(Client entity)
    {
        if (entity == null)
        {
            return;
        }

        await using var context = factory.Create();
        await context.Database.ExecuteSqlAsync($"delete from Clients where Id = {entity.Id}");
    }

    public async Task<int> RevenueOfClient(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<int>(
                $"""
                 select coalesce(sum(otb.Count * otb.Price), 0) as Value
                 from Orders o
                 join Clients c on o.ClientId = c.Id
                 join OrdersToBooks otb on o.Id = otb.OrderId
                 where c.IsDeleted = 0 and o.ClientId = {client.Id}
                 """)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<Client>> MostRevenueClients(int topCount = 10)
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<Client>(
                $"""
                 select top({topCount}) c.Id, c.FirstName, c.LastName, c.Phone, c.Gender, c.IsDeleted
                 from Clients c
                 where c.IsDeleted = 0
                 order by (coalesce((select coalesce(sum(otb.Count * otb.Price), 0)
                                     from OrdersToBooks otb
                                     join Orders o on otb.OrderId = o.Id
                                     join Clients cc on cc.Id = o.ClientId
                                     where cc.Id = c.Id), 0)
                 ) desc, c.Id
                 """)
            .ToListAsync();
    }
}