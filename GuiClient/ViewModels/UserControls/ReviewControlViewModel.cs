using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;

namespace GuiClient.ViewModels.UserControls;

public class ReviewControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<ReviewDataViewModel>(securityContext)
{
    protected override IDtoProvider<ReviewDataViewModel> GetProvider(string filterName)
    {
        return null;
    }
}