using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;
using DatabaseClient.Extensions;
using DatabaseClient.Providers;

namespace DatabaseClient.Models;

[Serializable]
public sealed class DbPrincipal : IPrincipalProvider, IDisposable
{
    private SecureString _password;

    public string Name { get; set; }

    public string RoleString
    {
        get => $"bsbd_{Role.ToString().ToLowerInvariant()}_role";
        set
        {
            var role = Role.Unknown;

            if (Enum.TryParse(typeof(Role), value?.Split("_")[1], true, out var parsedRole))
            {
                role = (Role)parsedRole;
            }

            Role = role;
        }
    }

    [NotMapped]
    public Role Role { get; set; }

    [NotMapped]
    public SecureString SecurePassword
    {
        get => _password;
        set
        {
            _password?.Dispose();
            _password = value;
        }
    }

    [NotMapped]
    public string Password => SecurePassword.Unsecure();

    public void Dispose()
    {
        _password?.Dispose();
    }

    public int Id { get; set; }

    public DbPrincipal GetPrincipal()
    {
        return this;
    }

    public override string ToString()
    {
        return $"{Role}/{Name}";
    }
}