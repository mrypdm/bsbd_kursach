using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.Data.Providers;

namespace GuiClient.ViewModels.UserControls;

public class BookInOrderControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<BookInOrderDataViewModel>(securityContext)
{
    protected override IDataViewModelProvider<BookInOrderDataViewModel> GetProvider(string filterName)
    {
        return null;
    }
}