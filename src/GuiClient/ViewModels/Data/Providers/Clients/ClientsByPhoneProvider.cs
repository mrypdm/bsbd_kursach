using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Data.Providers.Clients;

public class ClientsByPhoneProvider : IDataViewModelProvider<ClientDataViewModel>
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

    public async Task<ICollection<ClientDataViewModel>> GetAllAsync()
    {
        var clients = await _clientsRepository.GetClientsByPhoneAsync(_phone);
        return [_mapper.Map<ClientDataViewModel>(clients)];
    }

    public Task<ClientDataViewModel> CreateNewAsync()
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