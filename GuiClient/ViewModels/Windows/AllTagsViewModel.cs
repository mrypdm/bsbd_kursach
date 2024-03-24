using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories;
using GuiClient.Contexts;
using GuiClient.Dtos;
using GuiClient.Factories;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllTagsViewModel : AllEntitiesViewModel<Tag, TagDto>
{
    private readonly string _filter;
    private readonly TagsRepository _tagsRepository;
    private readonly object _value;

    public AllTagsViewModel(ISecurityContext securityContext, TagsRepository tagsRepository, IMapper mapper,
        DtoViewFactory dtoFactory, string filter, object value)
        : base(securityContext, tagsRepository, mapper, dtoFactory)
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
        AddText(window, nameof(TagDto.Id), true);
        AddText(window, nameof(TagDto.Name));
    }

    public override async Task RefreshAsync()
    {
        if (_filter is null)
        {
            await base.RefreshAsync();
            return;
        }

        var entity = _filter switch
        {
            "name" => await _tagsRepository.GetTagByNameAsync((string)_value),
            _ => throw new InvalidOperationException("Cannot determine parameter for filter")
        };

        Entities = [Mapper.Map<TagDto>(entity)];
    }

    protected override async Task UpdateAsync([NotNull] TagDto dto)
    {
        if (dto.Id == -1)
        {
            var tag = await _tagsRepository.AddTagAsync(dto.Name);
            MessageBox.Show($"Tag created with ID={tag.Id}");
        }
        else
        {
            await _tagsRepository.UpdateAsync(new Tag
            {
                Id = dto.Id,
                Name = dto.Name
            });
        }

        await RefreshAsync();
    }
}