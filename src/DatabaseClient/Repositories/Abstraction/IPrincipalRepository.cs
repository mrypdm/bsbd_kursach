using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IPrincipalRepository : IRepository<Principal>
{
    Task<Principal> GetByName(string name);

    Task<Principal> CreatePrincipalAsync(string name, SecureString password, Role role);

    Task ChangePasswordAsync(Principal principal, SecureString newPassword);

    Task ChangePasswordForceAsync(Principal principal);
}