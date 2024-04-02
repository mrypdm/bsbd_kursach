using DatabaseClient.Models;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data.Providers;
using GuiClient.ViewModels.Data.Providers.Principals;

namespace GuiClient.ViewModels.UserControls;

public class PrincipalControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Principal>(securityContext)
{
    protected override IDataViewModelProvider<Principal> GetProvider(string filterName)
    {
        return filterName switch
        {
            null => AllPrincipalsProvider.Create(),
            "name" => PrincipalByNameProvider.Create(),
            _ => throw InvalidFilter(filterName)
        };
    }
}