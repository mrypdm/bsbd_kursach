using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using DatabaseClient.Extensions;
using DatabaseClient.Providers;

namespace DatabaseClient.Principals;

[SuppressMessage("Naming", "CA1724:Type names should not match namespaces")]
public sealed class Principal : IDisposable, IPrincipalProvider, IPrincipal
{
    public Principal(string name, SecureString password, Role role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        SecurePassword = password;
        Role = role;
    }

    public string Name { get; }

    public string Password => SecurePassword?.Unsecure();

    public SecureString SecurePassword { get; }

    public Role Role { get; }

    public void Dispose()
    {
        SecurePassword?.Dispose();
    }

    public Principal GetPrincipal()
    {
        return this;
    }

    public override string ToString()
    {
        return $"{Role}/{Name}";
    }
}