using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class ReportsUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);