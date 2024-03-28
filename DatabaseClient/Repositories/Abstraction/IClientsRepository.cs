using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IClientsRepository : IRepository<Client>
{
    Task<Client> GetClientsByPhoneAsync(string phone);

    Task<ICollection<Client>> GetClientsByNameAsync(string firstName, string lastName);

    Task<ICollection<Client>> GetClientsByGender(Gender gender);

    Task<Client> AddClientAsync(string firstName, string lastName, string phone, Gender gender);

    Task<int> RevenueOfClient(Client client);

    Task<ICollection<Client>> MostRevenueClients(int topCount = 10);
}