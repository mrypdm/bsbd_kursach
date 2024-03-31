namespace DatabaseClient.Models.Internal;

public class DbTag : IDbEntity
{
    public int Id { get; set; }

    public string Name { get; set; }
}