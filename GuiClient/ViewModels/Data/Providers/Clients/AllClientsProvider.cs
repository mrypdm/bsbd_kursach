using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Data.Providers.Clients;

public class AllClientsProvider : IDataViewModelProvider<ClientDataViewModel>
{
    private readonly IClientsRepository _clientsRepository;
    private readonly IMapper _mapper;

    private AllClientsProvider(IClientsRepository clientsRepository, IMapper mapper)
    {
        _clientsRepository = clientsRepository;
        _mapper = mapper;
    }

    public async Task<ICollection<ClientDataViewModel>> GetAllAsync()
    {
        var clients = await _clientsRepository.GetAllAsync();
        return _mapper.Map<ClientDataViewModel[]>(clients);
    }

    public Task<ClientDataViewModel> CreateNewAsync()
    {
        return Task.FromResult(new ClientDataViewModel
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