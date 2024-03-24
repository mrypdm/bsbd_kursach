using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class TagsUserControlViewModel(ISecurityContext securityContext) : AuthenticatedViewModel(securityContext);