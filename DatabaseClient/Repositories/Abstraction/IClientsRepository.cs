using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IClientsRepository : IRepository<Client>
{
    // can return many when clients is deleted and phone is 0000000000
    Task<ICollection<Client>> GetClientsByPhoneAsync(string phone);

    Task<ICollection<Client>> GetClientsByNameAsync(string firstName, string lastName);

    Task<ICollection<Client>> GetClientsByGender(Gender gender);

    Task<Client> AddClientAsync(string firstName, string lastName, string phone, Gender gender);
}