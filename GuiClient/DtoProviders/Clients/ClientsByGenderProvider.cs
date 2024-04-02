using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Dto;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Clients;

public class ClientsByGenderProvider : IDtoProvider<ClientDto>
{
    private readonly IClientsRepository _clientsRepository;
    private readonly IMapper _mapper;
    private readonly Gender _gender;

    private ClientsByGenderProvider(IClientsRepository clientsRepository, IMapper mapper, Gender gender)
    {
        _clientsRepository = clientsRepository;
        _mapper = mapper;
        _gender = gender;
    }

    public async Task<ICollection<ClientDto>> GetAllAsync()
    {
        var clients = await _clientsRepository.GetClientsByGender(_gender);
        return _mapper.Map<ClientDto[]>(clients);
    }

    public Task<ClientDto> CreateNewAsync()
    {
        throw new NotSupportedException();
    }

    public bool CanCreate => false;

    public static ClientsByGenderProvider Create()
    {
        return AskerWindow.TryAskEnum<Gender>("Enter gender (Male|Female)", out var gender)
            ? new ClientsByGenderProvider(
                App.ServiceProvider.GetRequiredService<IClientsRepository>(),
                App.ServiceProvider.GetRequiredService<IMapper>(),
                gender)
            : null;
    }
}