using System;

namespace DatabaseClient.Models.Internal;

[Serializable]
public class DbTag : IDbEntity<Tag>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Tag ToEntity()
    {
        return new Tag
        {
            Id = Id,
            Name = Name
        };
    }
}