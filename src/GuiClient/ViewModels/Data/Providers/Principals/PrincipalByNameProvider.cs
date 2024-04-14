using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Data.Providers.Principals;

public class PrincipalByNameProvider : IDataViewModelProvider<Principal>
{
    private readonly IPrincipalRepository _principalRepository;
    private readonly string _name;

    private PrincipalByNameProvider(IPrincipalRepository principalRepository, string name)
    {
        _principalRepository = principalRepository;
        _name = name;
    }

    public async Task<ICollection<Principal>> GetAllAsync()
    {
        return [await _principalRepository.GetByName(_name)];
    }

    public Task<Principal> CreateNewAsync()
    {
        throw new NotSupportedException();
    }

    public bool CanCreate => false;

    public string Name => $"Principals with name '{_name}'";

    public static PrincipalByNameProvider Create()
    {
        return AskerWindow.TryAskString("Enter name", out var name)
            ? new PrincipalByNameProvider(App.ServiceProvider.GetRequiredService<IPrincipalRepository>(), name)
            : null;
    }
}