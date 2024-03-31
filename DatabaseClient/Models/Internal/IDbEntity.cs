namespace DatabaseClient.Models.Internal;

public interface IDbEntity<out TEntity>
{
    public TEntity ToEntity();
}