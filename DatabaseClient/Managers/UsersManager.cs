using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Managers;

[SuppressMessage("Security", "EF1002:Risk of vulnerability to SQL injection.")]
public class UsersManager
{
    private const string ForbiddenCharacters = "[]'\";";

    private static void ValidateCredentials(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Intersect(ForbiddenCharacters).Any())
        {
            throw new ArgumentException($"{name} cannot contains characters: {ForbiddenCharacters}");
        }
    }

    private static string GetRoleString(Role role)
    {
        return $"bsbd_{role.ToString().ToLowerInvariant()}_role";
    }

    public async Task CreateUserAsync(string userName, string password, Role role)
    {
        ValidateCredentials("User name", userName);
        ValidateCredentials("Password", password);

        var roleString = GetRoleString(role);
        var context = DatabaseContext.Instance;

        await context.Database
            .ExecuteSqlRawAsync($"CREATE USER [{userName}] WITH PASSWORD=N'{password}'")
            ;
        await context.Database
            .ExecuteSqlRawAsync($"ALTER ROLE [{roleString}] ADD MEMBER [{userName}]")
            ;
    }

    public async Task RemoveUserAsync(string userName)
    {
        ValidateCredentials("User name", userName);

        var context = DatabaseContext.Instance;
        await context.Database
            .ExecuteSqlRawAsync($"DROP USER [{userName}]")
            ;
    }

    public async Task ChangePasswordAsync(string userName, string newPassword, string oldPassword)
    {
        ValidateCredentials("User name", userName);
        ValidateCredentials("Password", newPassword);
        ValidateCredentials("Old password", oldPassword);

        var context = DatabaseContext.Instance;
        await context.Database
            .ExecuteSqlRawAsync($"ALTER USER [{userName}] WITH PASSWORD=N'{newPassword}' OLD_PASSWORD=N'{oldPassword}'")
            ;
    }

    // Current context should be authorized with ALTER ANY USER rights
    public async Task ForceChangePasswordAsync(string userName, string newPassword)
    {
        ValidateCredentials("User name", userName);
        ValidateCredentials("Password", newPassword);

        var context = DatabaseContext.Instance;
        await context.Database
            .ExecuteSqlRawAsync($"ALTER USER [{userName}] WITH PASSWORD=N'{newPassword}'")
            ;
    }

    public async Task<Role> GetUserRoleAsync(string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);

        var context = DatabaseContext.Instance;

        var roles = await context.Database
            .SqlQuery<string>($"exec bsbd_get_user_roles {userName}")
            .ToListAsync()
            ;

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