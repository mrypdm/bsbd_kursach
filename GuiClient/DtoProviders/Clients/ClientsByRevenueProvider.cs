using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Dto;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Clients;

public class ClientsByRevenueProvider : IDtoProvider<ClientDto>
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

    public async Task<ICollection<ClientDto>> GetAllAsync()
    {
        var clients = await _clientsRepository.MostRevenueClients(_count);
        return _mapper.Map<ClientDto[]>(clients);
    }

    public Task<ClientDto> CreateNewAsync()
    {
        throw new NotSupportedException();
    }

    public bool CanCreate => false;

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