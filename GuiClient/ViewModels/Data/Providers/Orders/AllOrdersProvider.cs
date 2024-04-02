using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Data.Providers.Orders;

public class AllOrdersProvider : IDataViewModelProvider<OrderDataViewModel>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IClientsRepository _clientsRepository;
    private readonly IMapper _mapper;

    private AllOrdersProvider(IOrdersRepository ordersRepository, IClientsRepository clientsRepository, IMapper mapper)
    {
        _ordersRepository = ordersRepository;
        _clientsRepository = clientsRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<OrderDataViewModel>> GetAllAsync()
    {
        var orders = await _ordersRepository.GetAllAsync();
        return _mapper.Map<OrderDataViewModel[]>(orders);
    }

    public async Task<OrderDataViewModel> CreateNewAsync()
    {
        if (!AskerWindow.TryAskInt("Enter client ID", out var clientId))
        {
            return null;
        }

        var client = await _clientsRepository.GetByIdAsync(clientId)
            ?? throw new KeyNotFoundException($"Cannot find client with Id={clientId}");

        return new OrderDataViewModel
        {
            Id = -1,
            ClientId = client.Id,
            Client = client.ToString(),
            CreatedAt = DateTime.Now
        };
    }

    public bool CanCreate => true;

    public string Name => "Orders";

    public static AllOrdersProvider Create()
    {
        return new AllOrdersProvider(
            App.ServiceProvider.GetRequiredService<IOrdersRepository>(),
            App.ServiceProvider.GetRequiredService<IClientsRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>());
    }
}