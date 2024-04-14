using System;

namespace GuiClient.ViewModels.Data;

[Serializable]
public sealed class OrderBookDataViewModel : NotifyPropertyChanged
{
    private int _count;

    public int OrderId { get; set; }

    public int BookId { get; set; }

    public string Book { get; set; }

    public int Count
    {
        get => _count;
        set
        {
            SetField(ref _count, value);
            OnPropertyChanged(nameof(TotalPrice));
        }
    }

    public int Price { get; set; }

    public int TotalPrice => Price * _count;
}