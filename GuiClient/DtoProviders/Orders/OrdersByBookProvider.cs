using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Dto;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Orders;

public class OrdersByBookProvider : IDtoProvider<OrderDto>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IClientsRepository _clientsRepository;
    private readonly IMapper _mapper;
    private readonly BookDto _book;

    private OrdersByBookProvider(IOrdersRepository ordersRepository,
        IClientsRepository clientsRepository,
        IMapper mapper,
        BookDto book)
    {
        _ordersRepository = ordersRepository;
        _clientsRepository = clientsRepository;
        _mapper = mapper;
        _book = book;
    }

    public async Task<ICollection<OrderDto>> GetAllAsync()
    {
        var orders = await _ordersRepository.GetOrdersForBookAsync(new Book { Id = _book.Id });
        return _mapper.Map<OrderDto[]>(orders);
    }

    public async Task<OrderDto> CreateNewAsync()
    {
        if (!AskerWindow.TryAskInt("Enter client ID", out var clientId))
        {
            return null;
        }

        var client = await _clientsRepository.GetByIdAsync(clientId)
            ?? throw new KeyNotFoundException($"Cannot find book with Id={clientId}");

        return new OrderDto
        {
            ClientId = client.Id,
            Client = client.ToString(),
            CreatedAt = DateTime.Now,
            Books =
            [
                new BookInOrderDto
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

    public static OrdersByBookProvider Create(BookDto book)
    {
        return new OrdersByBookProvider(
            App.ServiceProvider.GetRequiredService<IOrdersRepository>(),
            App.ServiceProvider.GetRequiredService<IClientsRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>(),
            book);
    }
}