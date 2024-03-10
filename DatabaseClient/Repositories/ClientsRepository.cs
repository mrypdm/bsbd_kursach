using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ClientsRepository
{
    public async Task<Client> GetClientByPhoneAsync(string phone)
    {
        var context = DatabaseContext.Instance;
        return await context.Clients
            .Where(m => m.Phone == phone)
            .SingleOrDefaultAsync()
            .ConfigureAwait(false);
    }
    
    public async Task<Client> GetClientByNameAsync(string firstName, string lastName)
    {
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

        return await command.SingleOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<Client> AddClientAsync(string firstName, string lastName, string phone)
    {
        var client = new Client
        {
            FirstName = firstName,
            LastName = lastName,
            Phone = phone
        };
        
        var context = DatabaseContext.Instance;
        var entity = await context.Clients.AddAsync(client).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity.Entity;
    }

    public async Task UpdateClientAsync(Client client)
    {
        var context = DatabaseContext.Instance;
        context.Update(client);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteClientAsync(Client client)
    {
        var context = DatabaseContext.Instance;
        context.Clients.Remove(client);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}