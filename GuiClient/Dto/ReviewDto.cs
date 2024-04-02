using System;

namespace GuiClient.Dto;

[Serializable]
public class ReviewDto
{
    public bool IsNew { get; set; }

    public int BookId { get; init; }

    public int ClientId { get; init; }

    public string Client { get; init; }

    public string Book { get; init; }

    public int Score { get; set; }

    public string Text { get; set; }
}