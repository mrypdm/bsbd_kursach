using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllReviewsViewModel(ISecurityContext securityContext, IReviewsRepository repository, IMapper mapper)
    : AllEntitiesViewModel<Review, ReviewDto>(securityContext, repository, mapper)
{
    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);

        if (IsAdmin)
        {
            AddButton(window, "Update", nameof(Update));
        }

        AddText(window, nameof(ReviewDto.BookId), true);
        AddText(window, nameof(ReviewDto.Book), true);
        AddText(window, nameof(ReviewDto.ClientId), true);
        AddText(window, nameof(ReviewDto.Client), true);
        AddText(window, nameof(ReviewDto.Score));
        AddText(window, nameof(ReviewDto.Text));
    }

    protected override async Task AddAsync()
    {
        var item = await DtoFactory();

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
        var review = await repository.GetByIdAsync(item.BookId, item.ClientId);

        if (review == null)
        {
            review = await repository.AddReviewAsync(
                new Client { Id = item.ClientId },
                new Book { Id = item.BookId },
                item.Score,
                item.Text);
            MessageBox.Show($"Review created for book with ID={review.BookId} for client with ID={review.ClientId}");
        }
        else
        {
            review.Score = item.Score;
            review.Text = item.Text;

            await repository.UpdateAsync(review);
        }

        await RefreshAsync();
    }
}