using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.Data.Providers.Books;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class TagWindowViewModel : AllEntitiesViewModel<Tag>
{
    private readonly ITagsRepository _tagsRepository;

    public TagWindowViewModel(ISecurityContext securityContext, ITagsRepository tagsRepository, IMapper mapper)
        : base(securityContext, mapper)
    {
        _tagsRepository = tagsRepository;

        ShowBooks = new AsyncFuncCommand<Tag>(ShowBooksAsync, item => item?.Id != -1);

        Add = new AsyncActionCommand(AddAsync, () => Provider?.CanCreate == true);
        Update = new AsyncFuncCommand<Tag>(UpdateAsync);
        Delete = new AsyncFuncCommand<Tag>(DeleteAsync, item => item?.Id == -1 || IsAdmin);

        Columns =
        [
            Button("Delete", nameof(Delete)),
            Button("Update", nameof(Update)),
            Button("Show books", nameof(ShowBooks)),
            Text(nameof(Tag.Id), true),
            Text(nameof(Tag.Name))
        ];
    }

    public ICommand ShowBooks { get; }

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
        var allReviews = App.ServiceProvider.GetRequiredService<IEntityViewModel<BookDataViewModel>>();
        await allReviews.ShowBy(BooksByTagsProvider.Create(item.Name));
    }
}