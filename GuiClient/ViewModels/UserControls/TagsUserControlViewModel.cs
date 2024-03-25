using DatabaseClient.Models;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Windows;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class TagsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<AllTagsViewModel, Tag, Tag>(securityContext)
{
    protected override object GetFilter(string filter)
    {
        return AskerWindow.AskString($"Enter {filter}");
    }
}