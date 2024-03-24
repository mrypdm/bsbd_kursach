using GuiClient.Contexts;

namespace GuiClient.ViewModels.UserControls;

public class PrincipalsUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);