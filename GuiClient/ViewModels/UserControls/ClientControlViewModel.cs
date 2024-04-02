using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.DtoProviders.Clients;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;

namespace GuiClient.ViewModels.UserControls;

public class ClientControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<ClientDataViewModel>(securityContext)
{
    protected override IDtoProvider<ClientDataViewModel> GetProvider(string filterName)
    {
        return filterName switch
        {
            null => AllClientsProvider.Create(),
            "phone" => ClientsByPhoneProvider.Create(),
            "name" => ClientsByNameProvider.Create(),
            "gender" => ClientsByGenderProvider.Create(),
            "revenue" => ClientsByRevenueProvider.Create(),
            _ => throw InvalidFilter(filterName)
        };
    }
}