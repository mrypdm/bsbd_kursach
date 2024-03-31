namespace DatabaseClient.Models.Internal;

public interface IDbEntity<TEntity>
{
    public TEntity ToEntity();
}