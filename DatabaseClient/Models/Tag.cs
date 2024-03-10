using System;
using System.Collections.Generic;

namespace DatabaseClient.Models;

[Serializable]
public class Tag : IEntity<int>
{
    public int Id { get; set; }

    public string Title { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
