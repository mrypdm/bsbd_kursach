using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.DtoProviders.Orders;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;

namespace GuiClient.ViewModels.UserControls;

public class OrderControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<OrderDataViewModel>(securityContext)
{
    protected override IDtoProvider<OrderDataViewModel> GetProvider(string filterName)
    {
        return AllOrdersProvider.Create();
    }
}