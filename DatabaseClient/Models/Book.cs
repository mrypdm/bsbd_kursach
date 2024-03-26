using System;
using System.Collections.Generic;

namespace DatabaseClient.Models;

[Serializable]
public class Book : IEntity
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public DateTime ReleaseDate { get; set; }

    public int Count { get; set; }

    public int Price { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<OrdersToBook> OrdersToBooks { get; set; } = new List<OrdersToBook>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}