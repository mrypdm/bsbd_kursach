using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.ViewModels.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Orders;

public class OrdersByClientProvider : IDtoProvider<OrderDataViewModel>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IMapper _mapper;
    private readonly ClientDataViewModel _client;

    private OrdersByClientProvider(IOrdersRepository ordersRepository, IMapper mapper, ClientDataViewModel client)
    {
        _ordersRepository = ordersRepository;
        _mapper = mapper;
        _client = client;
    }

    public async Task<ICollection<OrderDataViewModel>> GetAllAsync()
    {
        var orders = await _ordersRepository.GetOrdersForClientAsync(new Client { Id = _client.Id });
        return _mapper.Map<OrderDataViewModel[]>(orders);
    }

    public Task<OrderDataViewModel> CreateNewAsync()
    {
        return Task.FromResult(new OrderDataViewModel
        {
            Id = -1,
            ClientId = _client.Id,
            Client = _client.ToString(),
            CreatedAt = DateTime.Now
        });
    }

    public bool CanCreate => true;

    public string Name => $"Orders of client '{_client}'";

    public static OrdersByClientProvider Create(ClientDataViewModel client)
    {
        return new OrdersByClientProvider(
            App.ServiceProvider.GetRequiredService<IOrdersRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>(),
            client);
    }
}