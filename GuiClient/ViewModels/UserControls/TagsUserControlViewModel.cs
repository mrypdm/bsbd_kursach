using GuiClient.Contexts;

namespace GuiClient.ViewModels.UserControls;

public class TagsUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);