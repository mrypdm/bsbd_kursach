using DatabaseClient.Models;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.DtoProviders;
using GuiClient.DtoProviders.Clients;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class ClientsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Client, ClientDto>(securityContext)
{
    protected override IDtoProvider<ClientDto> GetProvider(string filterName)
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