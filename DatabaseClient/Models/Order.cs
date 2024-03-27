using System;
using System.Collections.Generic;

namespace DatabaseClient.Models;

[Serializable]
public class Order
{
    public int ClientId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Client Client { get; set; }

    public virtual ICollection<OrdersToBook> OrdersToBooks { get; set; } = new List<OrdersToBook>();

    public int Id { get; set; }
}