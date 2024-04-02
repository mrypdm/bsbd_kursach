using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.Data.Providers;
using GuiClient.ViewModels.Data.Providers.Orders;

namespace GuiClient.ViewModels.UserControls;

public class OrderControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<OrderDataViewModel>(securityContext)
{
    protected override IDataViewModelProvider<OrderDataViewModel> GetProvider(string filterName)
    {
        return AllOrdersProvider.Create();
    }
}