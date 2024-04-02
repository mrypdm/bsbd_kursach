using System;

namespace DatabaseClient.Models;

[Serializable]
public class Review
{
    public int ClientId { get; set; }

    public int BookId { get; set; }

    public int Score { get; set; }

    public string Text { get; set; }

    public Book Book { get; set; }

    public Client Client { get; set; }
}