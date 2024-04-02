using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Helpers;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;

namespace GuiClient.ViewModels.Windows;

public class ReviewWindowViewModel : AllEntitiesViewModel<ReviewDataViewModel>
{
    private readonly IReviewsRepository _repository;

    public ReviewWindowViewModel(ISecurityContext securityContext, IReviewsRepository repository, IMapper mapper)
        : base(securityContext, mapper)
    {
        _repository = repository;

        Add = new AsyncActionCommand(AddAsync, () => Provider?.CanCreate == true);
        Update = new AsyncFuncCommand<ReviewDataViewModel>(UpdateAsync, item => item?.IsNew == true || IsAdmin);
        Delete = new AsyncFuncCommand<ReviewDataViewModel>(DeleteAsync, item => item?.IsNew == true || IsAdmin);

        Columns =
        [
            ColumnHelper.CreateButton("Delete", nameof(Delete)),
            ColumnHelper.CreateButton("Update", nameof(Update)),
            ColumnHelper.CreateText(nameof(ReviewDataViewModel.BookId), true),
            ColumnHelper.CreateText(nameof(ReviewDataViewModel.Book), true),
            ColumnHelper.CreateText(nameof(ReviewDataViewModel.ClientId), true),
            ColumnHelper.CreateText(nameof(ReviewDataViewModel.Client), true),
            ColumnHelper.CreateText(nameof(ReviewDataViewModel.Score)),
            ColumnHelper.CreateText(nameof(ReviewDataViewModel.Text))
        ];
    }

    protected override async Task AddAsync()
    {
        var item = await Provider.CreateNewAsync();

        if (item == null)
        {
            return;
        }

        var currentItem = Entities
            .FirstOrDefault(m => m.BookId == item.BookId && m.ClientId == item.ClientId);

        if (currentItem is null)
        {
            Entities.Add(item);
            SelectedItem = item;
            return;
        }

        SelectedItem = currentItem;
    }

    protected override async Task UpdateAsync([NotNull] ReviewDataViewModel item)
    {
        if (item.IsNew)
        {
            var review = await _repository.AddReviewAsync(
                new Client { Id = item.ClientId },
                new Book { Id = item.BookId },
                item.Score,
                item.Text);
            MessageBox.Show($"Review created for book with ID={review.BookId} for client with ID={review.ClientId}");
        }
        else
        {
            await _repository.UpdateAsync(new Review
            {
                BookId = item.BookId,
                ClientId = item.ClientId,
                Score = item.Score,
                Text = item.Text
            });
        }

        await RefreshAsync();
    }

    protected override async Task DeleteAsync([NotNull] ReviewDataViewModel item)
    {
        if (item.IsNew)
        {
            Entities.Remove(item);
            return;
        }

        await _repository.RemoveAsync(new Review { BookId = item.BookId, ClientId = item.ClientId });
        await RefreshAsync();
    }
}