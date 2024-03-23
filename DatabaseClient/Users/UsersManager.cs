using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Users;

[SuppressMessage("Security", "EF1002:Risk of vulnerability to SQL injection.")]
public class UsersManager(DatabaseContextFactory factory)
{
    private static string GetRoleString(Role role)
    {
        return $"bsbd_{role.ToString().ToLowerInvariant()}_role";
    }

    public async Task CreateUserAsync(string userName, string password, Role role)
    {
        userName.ValidateForSqlInjection();
        password.ValidateForSqlInjection();

        var roleString = GetRoleString(role);
        await using var context = factory.Create();

        await context.Database
            .ExecuteSqlRawAsync($"CREATE USER [{userName}] WITH PASSWORD=N'{password}'");
        await context.Database
            .ExecuteSqlRawAsync($"ALTER ROLE [{roleString}] ADD MEMBER [{userName}]");
    }

    public async Task RemoveUserAsync(string userName)
    {
        userName.ValidateForSqlInjection();

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlRawAsync($"DROP USER [{userName}]");
    }

    public async Task ChangePasswordAsync(string userName, string password, string newPassword)
    {
        userName.ValidateForSqlInjection();
        newPassword.ValidateForSqlInjection();
        password.ValidateForSqlInjection();

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlRawAsync(
                $"ALTER USER [{userName}] WITH PASSWORD=N'{newPassword}' OLD_PASSWORD=N'{password}'");
    }

    // Current context should be authorized with ALTER ANY USER rights
    public async Task ForceChangePasswordAsync(string userName, string newPassword)
    {
        userName.ValidateForSqlInjection();
        newPassword.ValidateForSqlInjection();

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlRawAsync($"ALTER USER [{userName}] WITH PASSWORD=N'{newPassword}'");
    }

    public async Task<Role> GetUserRoleAsync(string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);

        await using var context = factory.Create();

        var roles = await context.Database
            .SqlQuery<string>($"exec bsbd_get_user_roles {userName}")
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