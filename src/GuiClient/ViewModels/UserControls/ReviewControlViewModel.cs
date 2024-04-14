using System;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.Data.Providers;

namespace GuiClient.ViewModels.UserControls;

public class ReviewControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<ReviewDataViewModel>(securityContext)
{
    protected override IDataViewModelProvider<ReviewDataViewModel> GetProvider(string filterName)
    {
        throw new NotSupportedException();
    }
}