using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;

namespace GuiClient.ViewModels.UserControls;

public class BookInOrderControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<BookInOrderDataViewModel>(securityContext)
{
    protected override IDtoProvider<BookInOrderDataViewModel> GetProvider(string filterName)
    {
        return null;
    }
}