using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Dto;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Clients;

public class ClientsByNameProvider : IDtoProvider<ClientDto>
{
    private readonly IClientsRepository _clientsRepository;
    private readonly IMapper _mapper;
    private readonly string _firstName;
    private readonly string _lastName;

    private ClientsByNameProvider(IClientsRepository clientsRepository,
        IMapper mapper,
        string firstName,
        string lastName)
    {
        _clientsRepository = clientsRepository;
        _mapper = mapper;
        _firstName = firstName;
        _lastName = lastName;
    }

    public async Task<ICollection<ClientDto>> GetAllAsync()
    {
        var clients = await _clientsRepository.GetClientsByNameAsync(_firstName, _lastName);
        return _mapper.Map<ClientDto[]>(clients);
    }

    public Task<ClientDto> CreateNewAsync()
    {
        throw new NotSupportedException();
    }

    public bool CanCreate => false;

    public string Name => $"Clients with name '{_firstName}, {_lastName}'";

    public static ClientsByNameProvider Create()
    {
        if (!AskerWindow.TryAskString("Enter name in format 'First Name, Last Name'", out var name))
        {
            return null;
        }

        var names = name.Split(",", StringSplitOptions.TrimEntries);

        return new ClientsByNameProvider(
            App.ServiceProvider.GetRequiredService<IClientsRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>(),
            names.ElementAtOrDefault(0),
            names.ElementAtOrDefault(1));
    }
}