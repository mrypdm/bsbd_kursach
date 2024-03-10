using System;

namespace DatabaseClient.Models;

[Serializable]
public class OrdersToBook
{
    public int OrderId { get; set; }

    public int BookId { get; set; }

    public int Count { get; set; }

    public virtual Book Book { get; set; }

    public virtual Order Order { get; set; }
}
