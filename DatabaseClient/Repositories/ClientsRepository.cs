using System;
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

    public async Task<Client> GetClientByPhoneAsync(string phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        await using var context = Factory.Create();
        return await context.Clients
            .Where(m => m.Phone == phone)
            .SingleOrDefaultAsync();
    }

    public async Task<Client> GetClientByNameAsync(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

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

        return await command.SingleOrDefaultAsync();
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
                .SetProperty(m => m.Phone, entity.Phone));
    }
}