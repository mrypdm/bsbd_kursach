using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.DtoProviders;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class ReviewControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<ReviewDto>(securityContext)
{
    protected override IDtoProvider<ReviewDto> GetProvider(string filterName)
    {
        return null;
    }
}