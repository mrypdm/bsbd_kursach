using GuiClient.Contexts;

namespace GuiClient.ViewModels.UserControls;

public class ReportsUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);