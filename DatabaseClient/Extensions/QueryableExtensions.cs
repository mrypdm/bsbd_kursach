using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Models.Internal;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Extensions;

public static class QueryableExtensions
{
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