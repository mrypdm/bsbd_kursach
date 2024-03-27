using System;
using DatabaseClient.Contexts;

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
}