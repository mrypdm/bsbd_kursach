using System;

namespace DatabaseClient.Models;

[Serializable]
public sealed class Order
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public DateTime CreatedAt { get; set; }

    public Client Client { get; set; }
}