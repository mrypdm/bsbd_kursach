using GuiClient.Contexts;

namespace GuiClient.ViewModels.UserControls;

public class UsersUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);