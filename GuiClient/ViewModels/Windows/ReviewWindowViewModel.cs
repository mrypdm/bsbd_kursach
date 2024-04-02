using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.Views.Windows;

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
    }

    public override void SetupDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        window.Clear();

        window.AddButton("Delete", nameof(Delete));
        window.AddButton("Update", nameof(Update));

        window.AddText(nameof(ReviewDataViewModel.BookId), true);
        window.AddText(nameof(ReviewDataViewModel.Book), true);
        window.AddText(nameof(ReviewDataViewModel.ClientId), true);
        window.AddText(nameof(ReviewDataViewModel.Client), true);
        window.AddText(nameof(ReviewDataViewModel.Score));
        window.AddText(nameof(ReviewDataViewModel.Text));
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