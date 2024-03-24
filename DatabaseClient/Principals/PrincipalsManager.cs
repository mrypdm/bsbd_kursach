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

    public async Task<Principal> CreatePrincipalAsync(string userName, SecureString password, Role role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentNullException.ThrowIfNull(password);

        await using var context = factory.Create();

        await context.Database
            .ExecuteSqlAsync($"exec bsbd_create_user {userName}, {password.Unsecure()}, {(int)role}");

        return new Principal(userName, password, role);
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

    public async Task<ICollection<IPrincipal>> GetAllPrincipalsAsync()
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<DatabasePrincipal>($"select * from bsbd_principals")
            .Select(m => m.ToPrincipal())
            .ToArrayAsync();
    }

    public async Task<IPrincipal> GetPrincipalByName(string name)
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<DatabasePrincipal>($"select * from bsbd_principals where PrincipalName = {name}")
            .Select(m => m.ToPrincipal())
            .SingleOrDefaultAsync();
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
            var role = Role.Unknown;

            if (Enum.TryParse(typeof(Role), PrincipalRole.Split("_")[1], true, out var parsedRole))
            {
                role = (Role)parsedRole;
            }

            return new Principal(PrincipalName, null, role);
        }
    }
}