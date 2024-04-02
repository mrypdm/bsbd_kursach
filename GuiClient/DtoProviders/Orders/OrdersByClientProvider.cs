using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Orders;

public class OrdersByClientProvider : IDtoProvider<OrderDto>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IMapper _mapper;
    private readonly ClientDto _client;

    private OrdersByClientProvider(IOrdersRepository ordersRepository, IMapper mapper, ClientDto client)
    {
        _ordersRepository = ordersRepository;
        _mapper = mapper;
        _client = client;
    }

    public async Task<ICollection<OrderDto>> GetAllAsync()
    {
        var orders = await _ordersRepository.GetOrdersForClientAsync(new Client { Id = _client.Id });
        return _mapper.Map<OrderDto[]>(orders);
    }

    public Task<OrderDto> CreateNewAsync()
    {
        return Task.FromResult(new OrderDto
        {
            Id = -1,
            ClientId = _client.Id,
            Client = _client.ToString(),
            CreatedAt = DateTime.Now
        });
    }

    public bool CanCreate => true;

    public static OrdersByClientProvider Create(ClientDto client)
    {
        return new OrdersByClientProvider(
            App.ServiceProvider.GetRequiredService<IOrdersRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>(),
            client);
    }
}