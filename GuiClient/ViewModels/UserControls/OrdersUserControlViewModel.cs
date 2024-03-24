using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class OrdersUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);