using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Dto;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Clients;

public class ClientsByPhoneProvider : IDtoProvider<ClientDto>
{
    private readonly IClientsRepository _clientsRepository;
    private readonly IMapper _mapper;
    private readonly string _phone;

    private ClientsByPhoneProvider(IClientsRepository clientsRepository, IMapper mapper, string phone)
    {
        _clientsRepository = clientsRepository;
        _mapper = mapper;
        _phone = phone;
    }

    public async Task<ICollection<ClientDto>> GetAllAsync()
    {
        var clients = await _clientsRepository.GetClientsByPhoneAsync(_phone);
        return [_mapper.Map<ClientDto>(clients)];
    }

    public Task<ClientDto> CreateNewAsync()
    {
        throw new NotSupportedException();
    }

    public bool CanCreate => false;

    public string Name => $"Clients with phone '{_phone}'";

    public static ClientsByPhoneProvider Create()
    {
        return AskerWindow.TryAskString("Enter phone in format '1234567890'", out var phone)
            ? new ClientsByPhoneProvider(
                App.ServiceProvider.GetRequiredService<IClientsRepository>(),
                App.ServiceProvider.GetRequiredService<IMapper>(),
                phone)
            : null;
    }
}