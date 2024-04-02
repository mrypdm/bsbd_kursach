﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.BooksInOrder;

public class BooksByOrderProvider : IDtoProvider<BookInOrderDataViewModel>
{
    private readonly IOrderBooksRepository _orderBooksRepository;
    private readonly IBooksRepository _booksRepository;
    private readonly IMapper _mapper;
    private readonly OrderDataViewModel _order;

    private BooksByOrderProvider(IOrderBooksRepository orderBooksRepository,
        IBooksRepository booksRepository,
        IMapper mapper,
        OrderDataViewModel order)
    {
        _orderBooksRepository = orderBooksRepository;
        _booksRepository = booksRepository;
        _mapper = mapper;
        _order = order;
    }

    public async Task<ICollection<BookInOrderDataViewModel>> GetAllAsync()
    {
        if (_order.Id == -1)
        {
            return _order.Books;
        }

        var books = await _orderBooksRepository.GetBooksForOrderAsync(new Order { Id = _order.Id });
        return _mapper.Map<BookInOrderDataViewModel[]>(books);
    }

    public async Task<BookInOrderDataViewModel> CreateNewAsync()
    {
        if (!AskerWindow.TryAskInt("Enter book ID", out var bookId))
        {
            return null;
        }

        var book = await _booksRepository.GetByIdAsync(bookId)
            ?? throw new KeyNotFoundException($"Cannot find book with Id={bookId}");

        return new BookInOrderDataViewModel
        {
            OrderId = -1,
            BookId = book.Id,
            Book = book.ToString(),
            Price = book.Price,
            Count = 1
        };
    }

    public bool CanCreate => _order.Id == -1;

    public string Name => $"Books in order with id '{_order.Id}'";

    public static BooksByOrderProvider Create(OrderDataViewModel order)
    {
        return new BooksByOrderProvider(
            App.ServiceProvider.GetRequiredService<IOrderBooksRepository>(),
            App.ServiceProvider.GetRequiredService<IBooksRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>(),
            order);
    }
}