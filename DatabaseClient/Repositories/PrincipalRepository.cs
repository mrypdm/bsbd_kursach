using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class PrincipalRepository(DbContextFactory factory) : IPrincipalRepository
{
    public async Task<Principal> GetByIdAsync(int id)
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<Principal>(
                $"""
                 select Id, Name, Role as RoleString
                 from bsbd_principals
                 where Id = {id}
                 """)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<Principal>> GetAllAsync()
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<Principal>($"select Id, Name, Role as RoleString from bsbd_principals")
            .ToListAsync();
    }

    public Task UpdateAsync(Principal entity)
    {
        throw new NotSupportedException();
    }

    public async Task RemoveAsync(Principal entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlAsync($"bsbd_delete_user {entity.Name}");
    }

    public async Task<Principal> CreatePrincipalAsync(string name, SecureString password, Role role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(password);

        await using var context = factory.Create();

        await context.Database
            .ExecuteSqlAsync($"exec bsbd_create_user {name}, {password.Unsecure()}, {(int)role}");

        return await GetByName(name);
    }

    public async Task ChangePasswordAsync(Principal principal, SecureString newPassword)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(newPassword);

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlAsync(
                $"exec bsbd_change_user_password {principal.Name}, {newPassword.Unsecure()}, {principal.Password}");
    }

    public async Task ChangePasswordForceAsync(Principal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlAsync($"exec bsbd_change_user_password {principal.Name}, {principal.Password}");
    }

    public async Task<Principal> GetByName(string name)
    {
        await using var context = factory.Create();
        return await context.Database
            .SqlQuery<Principal>(
                $"""
                 select Id, Name, Role as RoleString
                 from bsbd_principals
                 where Name = {name}
                 """)
            .SingleOrDefaultAsync();
    }
}