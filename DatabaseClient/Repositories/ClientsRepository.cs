using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ClientsRepository : BaseRepository<Client>
{
    public async Task<Client> GetClientByPhoneAsync(string phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        var context = DatabaseContext.Instance;
        return await context.Clients
            .Where(m => m.Phone == phone)
            .SingleOrDefaultAsync();
    }

    public async Task<Client> GetClientByNameAsync(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        var context = DatabaseContext.Instance;

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

    public async Task<Client> AddClientAsync(string firstName, string lastName, string phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        var client = new Client
        {
            FirstName = firstName,
            LastName = lastName,
            Phone = phone
        };

        var context = DatabaseContext.Instance;
        var entity = await context.Clients.AddAsync(client);
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    // TODO: as trigger
    public override async Task RemoveAsync(Client entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var context = DatabaseContext.Instance;

        var isUnPaidExists = await context.Orders
            .Where(m => m.ClientId == entity.Id && m.PaidAt == null)
            .AnyAsync();

        if (isUnPaidExists)
        {
            throw new InvalidOperationException("User has unpaid orders");
        }

        entity.FirstName = string.Empty;
        entity.LastName = string.Empty;
        entity.Phone = "0000000000";
        entity.IsDeleted = true;

        context.Update(entity);
        await context.SaveChangesAsync();
    }
}