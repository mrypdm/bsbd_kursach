using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.DtoProviders;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class BookInOrderControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<BookInOrderDto>(securityContext)
{
    protected override IDtoProvider<BookInOrderDto> GetProvider(string filterName)
    {
        return null;
    }
}