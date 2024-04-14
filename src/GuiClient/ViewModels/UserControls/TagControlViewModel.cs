using DatabaseClient.Models;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data.Providers;
using GuiClient.ViewModels.Data.Providers.Tags;

namespace GuiClient.ViewModels.UserControls;

public class TagControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Tag>(securityContext)
{
    protected override IDataViewModelProvider<Tag> GetProvider(string filterName)
    {
        return filterName switch
        {
            null => AllTagsProvider.Create(),
            "name" => TagByNameProvider.Create(),
            _ => throw InvalidFilter(filterName)
        };
    }
}