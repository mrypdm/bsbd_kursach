using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Data.Providers.Clients;

public class ClientsByRevenueProvider : IDataViewModelProvider<ClientDataViewModel>
{
    private readonly IClientsRepository _clientsRepository;
    private readonly IMapper _mapper;
    private readonly int _count;

    private ClientsByRevenueProvider(IClientsRepository clientsRepository, IMapper mapper, int count)
    {
        _clientsRepository = clientsRepository;
        _mapper = mapper;
        _count = count;
    }

    public async Task<ICollection<ClientDataViewModel>> GetAllAsync()
    {
        var clients = await _clientsRepository.MostRevenueClients(_count);
        return _mapper.Map<ClientDataViewModel[]>(clients);
    }

    public Task<ClientDataViewModel> CreateNewAsync()
    {
        throw new NotSupportedException();
    }

    public bool CanCreate => false;

    public string Name => $"Top {_count} clients by revenue";

    public static ClientsByRevenueProvider Create()
    {
        return AskerWindow.TryAskInt("Enter count", out var count)
            ? new ClientsByRevenueProvider(
                App.ServiceProvider.GetRequiredService<IClientsRepository>(),
                App.ServiceProvider.GetRequiredService<IMapper>(),
                count)
            : null;
    }
}