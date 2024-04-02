using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.DtoProviders.Books;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class AllTagsViewModel : AllEntitiesViewModel<Tag>
{
    private readonly ITagsRepository _tagsRepository;

    public AllTagsViewModel(ISecurityContext securityContext, ITagsRepository tagsRepository, IMapper mapper)
        : base(securityContext, mapper)
    {
        _tagsRepository = tagsRepository;

        ShowBooks = new AsyncFuncCommand<Tag>(ShowBooksAsync, item => item?.Id != -1);

        Add = new AsyncActionCommand(AddAsync, () => Provider?.CanCreate == true);
        Update = new AsyncFuncCommand<Tag>(UpdateAsync);
        Delete = new AsyncFuncCommand<Tag>(DeleteAsync, item => item?.Id == -1 || IsAdmin);
    }

    public ICommand ShowBooks { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        window.AddButton("Delete", nameof(Delete));
        window.AddButton("Update", nameof(Update));
        window.AddButton("Show books", nameof(ShowBooks));

        window.AddText(nameof(Tag.Id), true);
        window.AddText(nameof(Tag.Name));
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
            await _tagsRepository.UpdateAsync(item);
        }

        await RefreshAsync();
    }

    protected override async Task DeleteAsync([NotNull] Tag item)
    {
        if (item.Id == -1)
        {
            Entities.Remove(item);
            return;
        }

        await _tagsRepository.RemoveAsync(new Tag { Id = item.Id });
        await RefreshAsync();
    }

    private async Task ShowBooksAsync(Tag item)
    {
        var allReviews = App.ServiceProvider.GetRequiredService<IEntityViewModel<BookDto>>();
        await allReviews.ShowBy(BooksByTagsProvider.Create(item.Name));
    }
}