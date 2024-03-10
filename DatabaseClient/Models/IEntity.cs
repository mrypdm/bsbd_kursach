namespace DatabaseClient.Models;

public interface IEntity<TType>
{
    TType Id { get; set; }
}