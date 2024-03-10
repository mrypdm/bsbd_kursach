using System;
using System.Collections.Generic;

namespace DatabaseClient.Models;

[Serializable]
public class Order : IEntity<long>
{
    public long Id { get; set; }

    public int? ClientId { get; set; }

    public bool IsPaid { get; set; }

    public virtual Client Client { get; set; }

    public virtual ICollection<OrdersToBook> OrdersToBooks { get; set; } = new List<OrdersToBook>();
}