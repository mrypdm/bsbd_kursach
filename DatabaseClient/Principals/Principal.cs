using System;
using System.Net;
using System.Security;

namespace DatabaseClient.Principals;

public sealed class Principal : IDisposable
{
    public Principal(string name, SecureString password, Role role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(password);

        Role = role;
        Credential = new NetworkCredential(Name, SecurePassword);
    }

    public string Name => Credential.UserName;

    public string Password => Credential.Password;

    public SecureString SecurePassword => Credential.SecurePassword;

    public Role Role { get; }

    public NetworkCredential Credential { get; }

    public void Dispose()
    {
        SecurePassword?.Dispose();
    }

    public override string ToString()
    {
        return $"{Role}/{Name}";
    }
}