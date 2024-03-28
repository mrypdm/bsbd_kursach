using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class AllTagsViewModel : AllEntitiesViewModel<Tag, Tag>
{
    private readonly ITagsRepository _tagsRepository;

    public AllTagsViewModel(ISecurityContext securityContext, ITagsRepository tagsRepository, IMapper mapper)
        : base(securityContext, tagsRepository, mapper)
    {
        _tagsRepository = tagsRepository;

        ShowBooks = new AsyncFuncCommand<Tag>(ShowBooksAsync, item => item?.Id != -1);
    }

    public ICommand ShowBooks { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);

        if (IsWorker)
        {
            AddButton(window, "Update", nameof(Update));
            AddButton(window, "Show books", nameof(ShowBooks));
        }

        AddText(window, nameof(Tag.Id), true);
        AddText(window, nameof(Tag.Name));
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

    private async Task ShowBooksAsync(Tag item)
    {
        var allReviews = App.ServiceProvider.GetRequiredService<IEntityViewModel<Book, BookDto>>();

        await allReviews.ShowBy(
            r =>
            {
                var repo = r.Cast<Book, IBooksRepository>();
                return repo.GetBooksByTagsAsync([item.Name]);
            },
            () => Task.FromResult(new BookDto
            {
                Id = -1,
                Tags = item.Name
            }));
    }
}