using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;
using DatabaseClient.Extensions;
using DatabaseClient.Providers;

namespace DatabaseClient.Models;

[Serializable]
public sealed class DbPrincipal : IEntity, IPrincipalProvider, IDisposable
{
    private SecureString _password;

    public int Id { get; set; }

    public string Name { get; set; }

    [Column("Role")]
    public string RoleString { get; set; }

    [NotMapped]
    public Role Role
    {
        get
        {
            var role = Role.Unknown;

            if (Enum.TryParse(typeof(Role), RoleString.Split("_")[1], true, out var parsedRole))
            {
                role = (Role)parsedRole;
            }

            return role;
        }
    }

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

    public DbPrincipal GetPrincipal()
    {
        return this;
    }

    public override string ToString()
    {
        return $"{Role}/{Name}";
    }
}