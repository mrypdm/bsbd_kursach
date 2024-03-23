using GuiClient.Contexts;

namespace GuiClient.ViewModels.UserControls;

public class BooksToTagsUserControlViewModel(ISecurityContext securityContext)
    : AuthenticatedViewModel(securityContext);