using DatabaseClient.Models;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.DtoProviders;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class ReviewsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Review, ReviewDto>(securityContext)
{
    protected override IDtoProvider<ReviewDto> GetProvider(string filterName)
    {
        return null;
    }
}