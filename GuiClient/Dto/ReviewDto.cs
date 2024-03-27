using System;
using DatabaseClient.Models;

namespace GuiClient.Dto;

[Serializable]
public class ReviewDto : IEntity
{
    public int Id { get; set; } = -1;

    public int BookId { get; init; }

    public int ClientId { get; init; }

    public string Client { get; init; }

    public string Book { get; init; }

    public int Score { get; set; }

    public string Text { get; set; }
}