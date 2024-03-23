using GuiClient.Contexts;

namespace GuiClient.ViewModels.UserControls;

public class ClientsUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);