using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Principals;

public class PrincipalsManager(DatabaseContextFactory factory)
{
    private static string GetRoleString(Role role)
    {
        return $"bsbd_{role.ToString().ToLowerInvariant()}_role";
    }

    public async Task CreatePrincipalAsync(string userName, SecureString password, Role role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentNullException.ThrowIfNull(password);

        await using var context = factory.Create();

        await context.Database
            .ExecuteSqlAsync($"exec bsbd_create_user {userName}, {password.Unsecure()}, {(int)role}");
    }

    public async Task RemovePrincipalAsync(string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);

        await using var context = factory.Create();

        await context.Database
            .ExecuteSqlAsync($"bsbd_delete_user {userName}");
    }

    public async Task ChangePasswordAsync(string userName, SecureString password, SecureString newPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(newPassword);

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlAsync(
                $"exec bsbd_change_user_password {userName}, {newPassword.Unsecure()}, {password.Unsecure()}");
    }

    // Current context should be authorized with ALTER ANY USER rights
    public async Task ChangePasswordForceAsync(string userName, SecureString newPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentNullException.ThrowIfNull(newPassword);

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlAsync($"exec bsbd_change_user_password {userName}, {newPassword.Unsecure()}");
    }

    public async Task<ICollection<Principal>> GetAllPrincipalsAsync()
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<DatabasePrincipal>($"select * from bsbd_principals")
            .Select(m => m.ToPrincipal())
            .ToListAsync();
    }

    public async Task<Role> GetPrincipalRoleAsync(string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);

        await using var context = factory.Create();

        var roles = await context.Database
            .SqlQuery<string>($"select PrincipalRole from bsbd_principals where PrincipalName = {userName}")
            .ToListAsync();

        foreach (var role in (Role[])Enum.GetValues(typeof(Role)))
        {
            if (roles.Contains(GetRoleString(role)))
            {
                return role;
            }
        }

        return Role.Unknown;
    }

    [Serializable]
    [SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes")]
    private sealed class DatabasePrincipal
    {
        public int PrincipalId { get; set; }

        public string PrincipalName { get; set; }

        public string PrincipalRole { get; set; }

        public Principal ToPrincipal()
        {
            var role = Enum.Parse<Role>(PrincipalRole.Split("_")[1], true);
            return new Principal(PrincipalName, null, role);
        }
    }
}