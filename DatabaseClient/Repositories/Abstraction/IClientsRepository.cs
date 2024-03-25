using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IClientsRepository : IRepository<Client>
{
    Task<Client> GetClientByPhoneAsync(string phone);

    Task<Client> GetClientByNameAsync(string firstName, string lastName);

    Task<Client> AddClientAsync(string firstName, string lastName, string phone, Gender gender);
}