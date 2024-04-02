using DatabaseClient.Models;
using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.DtoProviders.Principals;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class PrincipalsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Principal>(securityContext)
{
    protected override IDtoProvider<Principal> GetProvider(string filterName)
    {
        return filterName switch
        {
            null => AllPrincipalsProvider.Create(),
            "name" => PrincipalByNameProvider.Create(),
            _ => throw InvalidFilter(filterName)
        };
    }
}