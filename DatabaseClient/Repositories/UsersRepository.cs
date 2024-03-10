using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

[SuppressMessage("Security", "EF1002:Risk of vulnerability to SQL injection.")]
public class UsersRepository
{
    private static void CheckCredentials(string userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new InvalidOperationException("User name cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Password cannot be empty");
        }

        if (userName.Contains(']'))
        {
            throw new InvalidOperationException("Password cannot contains ] character");
        }

        if (password.Contains('\''))
        {
            throw new InvalidOperationException("Password cannot contains ' character");
        }
    }

    private static string GetRoleString(Role role) => $"bsbd_{role.ToString().ToLowerInvariant()}_role";

    public async Task AddUserAsync(string userName, string password, Role role)
    {
        CheckCredentials(userName, password);

        var roleString = GetRoleString(role);
        var context = DatabaseContext.Instance;
        
        await context.Database.ExecuteSqlRawAsync($"CREATE USER [{userName}] WITH PASSWORD=N'{password}'");
        await context.Database.ExecuteSqlRawAsync($"ALTER ROLE [{roleString}] ADD MEMBER [{userName}]");
    }

    public async Task RemoveUserAsync(string userName)
    {
        CheckCredentials(userName, "default");
        var context = DatabaseContext.Instance;
        await context.Database.ExecuteSqlRawAsync($"DROP USER [{userName}]");
    }

    public async Task<Role> GetUserRoleAsync(string userName)
    {
        var context = DatabaseContext.Instance;

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