using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models.Internal;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Extensions;

public static class DatabaseContextExtensions
{
    public static void TryAttach<TEntity>(this DatabaseContext context, TEntity entity)
    {
        try
        {
            context?.Attach(entity);
        }
        catch (InvalidOperationException e)
            when (e.Message.Contains("is already being tracked", StringComparison.OrdinalIgnoreCase))
        {
            // nop
        }
    }

    private static async Task<IEnumerable<TEntity>> AsEnumerable<TDbEntity, TEntity>(this IQueryable<TDbEntity> query)
        where TDbEntity : IDbEntity<TEntity>, new()
    {
        ArgumentNullException.ThrowIfNull(query);
        var result = await query.ToListAsync();
        return result.Select(m => m.ToEntity());
    }

    public static async Task<List<TEntity>> AsListAsync<TDbEntity, TEntity>(this IQueryable<TDbEntity> query)
        where TDbEntity : IDbEntity<TEntity>, new()
    {
        return (await query.AsEnumerable<TDbEntity, TEntity>()).ToList();
    }

    public static async Task<TEntity> SingleOrDefaultAsync<TDbEntity, TEntity>(this IQueryable<TDbEntity> query)
        where TDbEntity : IDbEntity<TEntity>, new()
    {
        return (await query.AsEnumerable<TDbEntity, TEntity>()).SingleOrDefault();
    }
}