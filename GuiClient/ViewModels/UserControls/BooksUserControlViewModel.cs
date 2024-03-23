using GuiClient.Contexts;

namespace GuiClient.ViewModels.UserControls;

public class BooksUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);