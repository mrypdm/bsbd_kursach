using System;

namespace DatabaseClient.Models;

[Serializable]
public sealed class OrderBook
{
    public int OrderId { get; set; }

    public int BookId { get; set; }

    public int Count { get; set; }

    public Book Book { get; set; }
}