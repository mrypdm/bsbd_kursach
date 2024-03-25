using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllTagsViewModel(ISecurityContext securityContext, ITagsRepository tagsRepository, IMapper mapper)
    : AllEntitiesViewModel<Tag, Tag>(securityContext, tagsRepository, mapper)
{
    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);
        AddText(window, nameof(Tag.Id), true);
        AddText(window, nameof(Tag.Name));
    }

    protected override async Task UpdateAsync([NotNull] Tag item)
    {
        if (item.Id == -1)
        {
            var tag = await tagsRepository.AddTagAsync(item.Name);
            MessageBox.Show($"Tag created with ID={tag.Id}");
        }
        else
        {
            await tagsRepository.UpdateAsync(new Tag
            {
                Id = item.Id,
                Name = item.Name
            });
        }

        await RefreshAsync();
    }
}