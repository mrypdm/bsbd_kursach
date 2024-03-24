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

public class AllTagsViewModel : AllEntitiesViewModel<Tag, Tag>
{
    private readonly string _filter;
    private readonly TagsRepository _tagsRepository;
    private readonly object _value;

    public AllTagsViewModel(ISecurityContext securityContext, TagsRepository tagsRepository, IMapper mapper,
        string filter, object value)
        : base(securityContext, tagsRepository, mapper)
    {
        _tagsRepository = tagsRepository;
        _filter = filter;
        _value = value;

        WindowTitlePostfix = _filter switch
        {
            null => string.Empty,
            "name" => $"with Name = {_value}",
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
        if (_filter is null)
        {
            await base.RefreshAsync();
            return;
        }

        Entities = _filter switch
        {
            "name" => [await _tagsRepository.GetTagByNameAsync((string)_value)],
            _ => throw new InvalidOperationException("Cannot determine parameter for filter")
        };
    }

    protected override async Task UpdateAsync([NotNull] Tag item)
    {
        if (item.Id == -1)
        {
            var tag = await _tagsRepository.AddTagAsync(item.Name);
            MessageBox.Show($"Tag created with ID={tag.Id}");
        }
        else
        {
            await _tagsRepository.UpdateAsync(new Tag
            {
                Id = item.Id,
                Name = item.Name
            });
        }

        await RefreshAsync();
    }
}