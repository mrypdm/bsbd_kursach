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
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllReviewsViewModel : AllEntitiesViewModel<ReviewDto>
{
    private readonly IReviewsRepository _repository;

    public AllReviewsViewModel(ISecurityContext securityContext, IReviewsRepository repository, IMapper mapper)
        : base(securityContext, mapper)
    {
        _repository = repository;

        Add = new AsyncActionCommand(AddAsync, () => Provider?.CanCreate == true);
        Update = new AsyncFuncCommand<ReviewDto>(UpdateAsync, item => item?.IsNew == true || IsAdmin);
        Delete = new AsyncFuncCommand<ReviewDto>(DeleteAsync, item => item?.IsNew == true || IsAdmin);
    }

    public override void SetupDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        window.Clear();

        window.AddButton("Delete", nameof(Delete));
        window.AddButton("Update", nameof(Update));

        window.AddText(nameof(ReviewDto.BookId), true);
        window.AddText(nameof(ReviewDto.Book), true);
        window.AddText(nameof(ReviewDto.ClientId), true);
        window.AddText(nameof(ReviewDto.Client), true);
        window.AddText(nameof(ReviewDto.Score));
        window.AddText(nameof(ReviewDto.Text));
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

    protected override async Task UpdateAsync([NotNull] ReviewDto item)
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

    protected override async Task DeleteAsync([NotNull] ReviewDto item)
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