using System;
using System.Net;
using System.Security;

namespace DatabaseClient.Principals;

public sealed class Principal(string name, SecureString password, Role role) : IDisposable
{
    public string Name { get; } = name;

    public SecureString Password { get; } = password;

    public Role Role { get; } = role;

    public NetworkCredential Credential { get; } = new(name, password);

    public void Dispose()
    {
        Password?.Dispose();
    }

    public override string ToString()
    {
        return $"{Role}/{Name}";
    }
}