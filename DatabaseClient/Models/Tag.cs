using System;
using System.Collections.Generic;

namespace DatabaseClient.Models;

[Serializable]
public class Tag : IEntity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}