using System;

namespace GuiClient.ViewModels.Data;

[Serializable]
public class ReviewDataViewModel
{
    public bool IsNew { get; set; }

    public int BookId { get; init; }

    public int ClientId { get; init; }

    public string Client { get; init; }

    public string Book { get; init; }

    public int Score { get; set; }

    public string Text { get; set; }
}