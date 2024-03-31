using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Converters;
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

    public static async Task<List<TEntity>> AsListAsync<TDbEntity, TEntity>(this IQueryable<TDbEntity> query,
        IGroupConverter<TDbEntity, TEntity> conv)
        where TDbEntity : IDbEntity, new()
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(conv);

        var result = await query.ToListAsync();

        return result
            .GroupBy(m => m.Id)
            .Select(conv.Convert)
            .ToList();
    }

    public static async Task<TEntity> SingleOrDefaultAsync<TDbEntity, TEntity>(this IQueryable<TDbEntity> query,
        IGroupConverter<TDbEntity, TEntity> conv)
        where TDbEntity : IDbEntity, new()
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(conv);

        var result = await query.ToListAsync();

        return result
            .GroupBy(m => m.Id)
            .Select(conv.Convert)
            .SingleOrDefault();
    }
}