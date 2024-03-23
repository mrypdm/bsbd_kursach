using System.Net;
using System.Security;

namespace DatabaseClient.Users;

public class User(string userName, SecureString password, Role role)
{
    public string UserName { get; } = userName;

    public SecureString Password { get; } = password;

    public Role Role { get; } = role;

    public NetworkCredential Credential => new(UserName, Password);

    public override string ToString()
    {
        return $"{Role}/{UserName}";
    }
}