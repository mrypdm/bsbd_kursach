using DatabaseClient.Models;
using GuiClient.Contexts;
using GuiClient.Factories;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class TagsUserControlViewModel(
    ISecurityContext securityContext,
    AllEntitiesWindowViewModelFactory factory)
    : EntityUserControlViewModel<Tag, Tag>(securityContext, factory)
{
    protected override object GetFilter(string filter)
    {
        return AskerWindow.AskString($"Enter {filter}");
    }
}