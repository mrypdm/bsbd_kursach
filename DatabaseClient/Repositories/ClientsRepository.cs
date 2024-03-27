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
            .Where(m => m.Id == id)
            .SingleOrDefaultAsync();
    }

    public override async Task<ICollection<Client>> GetAllAsync()
    {
        await using var context = Factory.Create();
        return await context.Clients
            .Where(m => !m.IsDeleted)
            .ToListAsync();
    }

    public async Task<ICollection<Client>> GetClientsByPhoneAsync(string phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        await using var context = Factory.Create();
        return await context.Clients
            .Where(m => m.Phone == phone)
            .ToListAsync();
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

        return await command.ToListAsync();
    }

    public async Task<ICollection<Client>> GetClientsByGender(Gender gender)
    {
        await using var context = Factory.Create();
        return await context.Clients
            .Where(m => m.Gender == gender && !m.IsDeleted)
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
}