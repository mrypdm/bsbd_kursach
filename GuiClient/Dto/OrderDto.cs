using System;

namespace GuiClient.Dto;

public sealed class OrderDto : NotifyPropertyChanged
{
    private int _totalSum;

    public int Id { get; set; } = -1;

    public int ClientId { get; set; }

    public string Client { get; set; }

    public DateTime CreatedAt { get; set; }

    public int TotalSum
    {
        get => _totalSum;
        set => SetField(ref _totalSum, value);
    }
}