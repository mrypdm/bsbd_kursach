using GuiClient.Contexts;

namespace GuiClient.ViewModels.UserControls;

public class ReviewsUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);