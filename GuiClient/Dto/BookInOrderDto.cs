using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GuiClient.Dto;

public sealed class BookInOrderDto : INotifyPropertyChanged
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

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }
}