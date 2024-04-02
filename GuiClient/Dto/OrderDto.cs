﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GuiClient.Dto;

public sealed class OrderDto : NotifyPropertyChanged
{
    private ICollection<BookInOrderDto> _books = [];
    private int? _totalSum;

    public int Id { get; set; } = -1;

    public int ClientId { get; set; }

    public string Client { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<BookInOrderDto> Books
    {
        get => _books;
        set
        {
            SetField(ref _books, value);
            TotalSum = _books.Sum(m => m.Count * m.Price);
        }
    }

    public int? TotalSum
    {
        get => _totalSum;
        set => SetField(ref _totalSum, value);
    }
}