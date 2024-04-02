using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.Data.Providers;
using GuiClient.ViewModels.Data.Providers.Clients;

namespace GuiClient.ViewModels.UserControls;

public class ClientControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<ClientDataViewModel>(securityContext)
{
    protected override IDataViewModelProvider<ClientDataViewModel> GetProvider(string filterName)
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