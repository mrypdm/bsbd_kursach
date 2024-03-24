using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class PrincipalsUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);