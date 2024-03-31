namespace DatabaseClient.Models.Internal;

public interface IDbEntity<TEntity>
{
    public int Id { get; set; }

    public TEntity ToEntity();
}