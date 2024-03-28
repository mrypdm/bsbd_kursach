using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ClientsRepository(DatabaseContextFactory factory) : BaseRepository<Client>(factory), IClientsRepository
{
    // Instead of delete, the record is updated by trigger bsbd_mark_user_deleted with the parameters:
    // FirstName = empty,
    // LastName = empty,
    // Phone = 0000000000,
    // IsDeleted = true

    public override async Task<Client> GetByIdAsync(int id)
    {
        await using var context = Factory.Create();
        return await context.Clients
            .Where(m => m.Id == id && !m.IsDeleted)
            .Include(m => m.Orders)
            .SingleOrDefaultAsync();
    }

    public override async Task<ICollection<Client>> GetAllAsync()
    {
        await using var context = Factory.Create();
        return await context.Clients
            .Where(m => !m.IsDeleted)
            .Include(m => m.Orders)
            .ToListAsync();
    }

    public async Task<Client> GetClientsByPhoneAsync(string phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        await using var context = Factory.Create();
        return await context.Clients
            .Where(m => m.Phone == phone && !m.IsDeleted)
            .Include(m => m.Orders)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<Client>> GetClientsByNameAsync(string firstName, string lastName)
    {
        await using var context = Factory.Create();

        var command = context.Clients as IQueryable<Client>;

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            command = command.Where(m => m.FirstName == firstName);
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            command = command.Where(m => m.LastName == lastName);
        }

        return await command.Include(m => m.Orders).ToListAsync();
    }

    public async Task<ICollection<Client>> GetClientsByGender(Gender gender)
    {
        await using var context = Factory.Create();
        return await context.Clients
            .Where(m => m.Gender == gender && !m.IsDeleted)
            .Include(m => m.Orders)
            .ToArrayAsync();
    }

    public async Task<Client> AddClientAsync(string firstName, string lastName, string phone, Gender gender)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        var client = new Client
        {
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            Gender = gender
        };

        await using var context = Factory.Create();
        var entity = await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    public override async Task UpdateAsync(Client entity)
    {
        await using var context = Factory.Create();
        await context.Clients
            .Where(m => m.Id == entity.Id)
            .ExecuteUpdateAsync(o => o
                .SetProperty(m => m.FirstName, entity.FirstName)
                .SetProperty(m => m.LastName, entity.LastName)
                .SetProperty(m => m.Phone, entity.Phone)
                .SetProperty(m => m.Gender, entity.Gender));
    }

    public async Task<int> RevenueOfClient(Client client)
    {
        await using var context = Factory.Create();
        return await context.Orders
            .Where(m => m.ClientId == client.Id && !m.Client.IsDeleted)
            .SelectMany(m => m.OrdersToBooks)
            .SumAsync(m => m.Count * m.Book.Price);
    }

    public async Task<ICollection<Client>> MostRevenueClients(int topCount = 10)
    {
        await using var context = Factory.Create();
        return await context.Clients
            .Where(m => !m.IsDeleted)
            .Include(m => m.Orders)
            .Select(m => new
            {
                Client = m,
                Sum = m.Orders.SelectMany(order
                        => order.OrdersToBooks.Select(orb => orb.Count * orb.Book.Price))
                    .Sum()
            })
            .OrderByDescending(m => m.Sum)
            .Take(topCount)
            .Select(m => m.Client)
            .ToListAsync();
    }
}