using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.DtoProviders.Clients;

public class AllClientsProvider : IDtoProvider<ClientDto>
{
    private readonly IClientsRepository _clientsRepository;
    private readonly IMapper _mapper;

    private AllClientsProvider(IClientsRepository clientsRepository, IMapper mapper)
    {
        _clientsRepository = clientsRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<ClientDto>> GetAllAsync()
    {
        var clients = await _clientsRepository.GetAllAsync();
        return _mapper.Map<ClientDto[]>(clients);
    }

    public Task<ClientDto> CreateNewAsync()
    {
        return Task.FromResult(new ClientDto
        {
            Id = -1
        });
    }

    public bool CanCreate => true;

    public string Name => "Clients";

    public static AllClientsProvider Create()
    {
        return new AllClientsProvider(
            App.ServiceProvider.GetRequiredService<IClientsRepository>(),
            App.ServiceProvider.GetRequiredService<IMapper>());
    }
}