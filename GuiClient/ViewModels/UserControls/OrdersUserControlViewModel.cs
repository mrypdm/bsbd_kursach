using GuiClient.Contexts;

namespace GuiClient.ViewModels.UserControls;

public class OrdersUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);