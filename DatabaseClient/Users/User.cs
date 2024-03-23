using System;
using System.Net;
using System.Security;

namespace DatabaseClient.Users;

public sealed class User(string userName, SecureString password, Role role) : IDisposable
{
    public string UserName { get; } = userName;

    public SecureString Password { get; } = password;

    public Role Role { get; } = role;

    public NetworkCredential Credential => new(UserName, Password);

    public void Dispose()
    {
        Password?.Dispose();
    }

    public override string ToString()
    {
        return $"{Role}/{UserName}";
    }
}