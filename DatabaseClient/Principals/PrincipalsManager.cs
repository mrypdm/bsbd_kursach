using System;
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

        await context.Database.ExecuteSqlAsync($"exec bsbd_create_user {userName} {password.Unsecure()} {(int)role}");
    }

    public async Task RemovePrincipalAsync(string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);

        await using var context = factory.Create();

        await context.Database.ExecuteSqlAsync($"exec bsbd_delete_user {userName}");
    }

    public async Task ChangePasswordAsync(string userName, SecureString newPassword, SecureString oldPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentNullException.ThrowIfNull(newPassword);
        ArgumentNullException.ThrowIfNull(oldPassword);

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlAsync(
                $"exec bsbd_change_user_password {userName} {newPassword.Unsecure()} {oldPassword.Unsecure()}");
    }

    // Current context should be authorized with ALTER ANY USER rights
    public async Task ChangePasswordForceAsync(string userName, SecureString newPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentNullException.ThrowIfNull(newPassword);

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlAsync($"exec bsbd_change_user_password {userName} {newPassword.Unsecure()}");
    }

    public async Task GetAllPrincipalsAsync()
    {
        await using var context = factory.Create();

        var roles = await context.Database
            .SqlQuery<string>($"select * from bsbd_principals")
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
}