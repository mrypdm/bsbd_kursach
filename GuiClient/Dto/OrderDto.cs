using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GuiClient.Dto;

public sealed class OrderDto : INotifyPropertyChanged
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