using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllTagsViewModel(ISecurityContext securityContext, TagsRepository tagsRepository, IMapper mapper)
    : AllEntitiesViewModel<Tag, Tag>(securityContext, tagsRepository, mapper)
{
    protected override void SetFilterInternal()
    {
        WindowTitlePostfix = Filter switch
        {
            null => string.Empty,
            "name" => $"with Name = {FilterValue}",
            _ => throw new InvalidOperationException("Cannot determine parameter for filter")
        };
    }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);
        AddText(window, nameof(Tag.Id), true);
        AddText(window, nameof(Tag.Name));
    }

    public override async Task RefreshAsync()
    {
        if (Filter is null)
        {
            await base.RefreshAsync();
            return;
        }

        Entities = Filter switch
        {
            "name" => [await tagsRepository.GetTagByNameAsync((string)FilterValue)],
            _ => throw new InvalidOperationException("Cannot determine parameter for filter")
        };
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