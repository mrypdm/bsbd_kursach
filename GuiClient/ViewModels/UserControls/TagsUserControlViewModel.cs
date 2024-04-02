using DatabaseClient.Models;
using GuiClient.Contexts;
using GuiClient.DtoProviders;
using GuiClient.DtoProviders.Tags;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class TagsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Tag, Tag>(securityContext)
{
    protected override IDtoProvider<Tag> GetProvider(string filterName)
    {
        return filterName switch
        {
            null => AllTagsProvider.Create(),
            "name" => TagByNameProvider.Create(),
            _ => throw InvalidFilter(filterName)
        };
    }
}