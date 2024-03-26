using System;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;

namespace DatabaseClient.Extensions;

public static class RepositoryExtensions
{
    public static TRepo Cast<TEntity, TRepo>(this IRepository<TEntity> repository)
        where TEntity : IEntity
    {
        if (repository is TRepo casted)
        {
            return casted;
        }

        throw new InvalidOperationException(
            $"Unexpected repository. Expected {typeof(TRepo).Name}, but was {repository?.GetType().Name ?? "<null>"}");
    }
}