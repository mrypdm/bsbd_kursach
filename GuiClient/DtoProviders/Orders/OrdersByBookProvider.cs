using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Orders;

public class OrdersByBookProvider : IDtoProvider<OrderDataViewModel>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IClientsRepository _clientsRepository;
    private readonly IMapper _mapper;
    private readonly BookDataViewModel _book;

    private OrdersByBookProvider(IOrdersRepository ordersRepository,
        IClientsRepository clientsRepository,
        IMapper mapper,
        BookDataViewModel book)
    {
        _ordersRepository = ordersRepository;
        _clientsRepository = clientsRepository;
        _mapper = mapper;
        _book = book;
    }

    public async Task<ICollection<OrderDataViewModel>> GetAllAsync()
    {
        var orders = await _ordersRepository.GetOrdersForBookAsync(new Book { Id = _book.Id });
        return _mapper.Map<OrderDataViewModel[]>(orders);
    }

    public async Task<OrderDataViewModel> CreateNewAsync()
    {
        if (!AskerWindow.TryAskInt("Enter client ID", out var clientId))
        {
            return null;
        }

        var client = await _clientsRepository.GetByIdAsync(clientId)
            ?? throw new KeyNotFoundException($"Cannot find book with Id={clientId}");

        return new OrderDataViewModel
        {
            ClientId = client.Id,
            Client = client.ToString(),
            CreatedAt = DateTime.Now,
            Books =
            [
                new BookInOrderDataViewModel
                {
                    OrderId = -1,
                    BookId = _book.Id,
                    Book = _book.ToString(),
                    Count = 1,
                    Price = _book.Price
                }
            ]
        };
    }

    public bool CanCreate => true;

    public string Name => $"Orders for book '{_book}'";

    public static OrdersByBookProvider Create(BookDataViewModel book)
    {
        return new OrdersByBookProvider(
            App.ServiceProvider.GetRequiredService<IOrdersRepository>(),
            App.ServiceProvider.GetRequiredService<IClientsRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>(),
            book);
    }
}