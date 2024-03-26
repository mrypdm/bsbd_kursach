using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IPrincipalRepository : IRepository<DbPrincipal>
{
    Task<DbPrincipal> GetByName(string name);

    Task<DbPrincipal> CreatePrincipalAsync(string name, SecureString password, Role role);

    Task ChangePasswordAsync(DbPrincipal principal, SecureString newPassword);

    Task ChangePasswordForceAsync(DbPrincipal principal);
}