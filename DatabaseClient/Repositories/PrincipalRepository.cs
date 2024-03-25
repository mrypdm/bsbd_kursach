﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class PrincipalRepository(DatabaseContextFactory factory) : IPrincipalRepository
{
    public async Task<DbPrincipal> GetByIdAsync(int id)
    {
        await using var context = factory.Create();
        return await context.Principals
            .Where(m => m.Id == id)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<DbPrincipal>> GetAllAsync()
    {
        await using var context = factory.Create();
        return await context.Principals.ToArrayAsync();
    }

    public Task UpdateAsync(DbPrincipal entity)
    {
        throw new NotSupportedException();
    }

    public async Task RemoveAsync(DbPrincipal entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlAsync($"bsbd_delete_user {entity.Name}");
    }

    public async Task<DbPrincipal> CreatePrincipalAsync(string name, SecureString password, Role role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(password);

        await using var context = factory.Create();

        await context.Database
            .ExecuteSqlAsync($"exec bsbd_create_user {name}, {password.Unsecure()}, {(int)role}");

        return await GetByName(name);
    }

    public async Task ChangePasswordAsync(DbPrincipal principal, SecureString newPassword)
    {
        ArgumentNullException.ThrowIfNull(principal);
        ArgumentNullException.ThrowIfNull(newPassword);

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlAsync(
                $"exec bsbd_change_user_password {principal.Name}, {newPassword.Unsecure()}, {principal.Password}");
    }

    public async Task ChangePasswordForceAsync(DbPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        await using var context = factory.Create();
        await context.Database
            .ExecuteSqlAsync($"exec bsbd_change_user_password {principal.Name}, {principal.Password}");
    }

    public async Task<DbPrincipal> GetByName(string name)
    {
        await using var context = factory.Create();
        return await context.Principals
            .Where(m => m.Name == name)
            .SingleOrDefaultAsync();
    }
}