using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseClient.Extensions;

public static class CollectionExtensions
{
    public static void RemoveWhere<TEntity>(this ICollection<TEntity> collection, Func<TEntity, bool> selector)
    {
        if (collection == null)
        {
            return;
        }

        var toDelete = collection.Where(selector).ToArray();
        foreach (var entity in toDelete)
        {
            collection.Remove(entity);
        }
    }
}