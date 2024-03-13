using System;

namespace DatabaseClient.Models;

[Serializable]
public class Review : IEntity
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public int BookId { get; set; }

    public int Score { get; set; }

    public string Text { get; set; }

    public virtual Book Book { get; set; }

    public virtual Client Client { get; set; }
}