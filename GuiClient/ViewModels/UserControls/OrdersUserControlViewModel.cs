using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.DtoProviders;
using GuiClient.DtoProviders.Orders;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class OrdersUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<OrderDto>(securityContext)
{
    protected override IDtoProvider<OrderDto> GetProvider(string filterName)
    {
        return AllOrdersProvider.Create();
    }
}