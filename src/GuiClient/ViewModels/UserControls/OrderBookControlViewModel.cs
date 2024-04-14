using System;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.Data.Providers;

namespace GuiClient.ViewModels.UserControls;

public class OrderBookControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<OrderBookDataViewModel>(securityContext)
{
    protected override IDataViewModelProvider<OrderBookDataViewModel> GetProvider(string filterName)
    {
        throw new NotSupportedException();
    }
}