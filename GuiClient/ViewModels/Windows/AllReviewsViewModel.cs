using System.Diagnostics.CodeAnalysis;
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
        AddText(window, nameof(ReviewDto.Id), true);
        AddText(window, nameof(ReviewDto.BookId), true);
        AddText(window, nameof(ReviewDto.Book), true);
        AddText(window, nameof(ReviewDto.ClientId), true);
        AddText(window, nameof(ReviewDto.Client), true);
        AddText(window, nameof(ReviewDto.Score));
        AddText(window, nameof(ReviewDto.Text));
    }

    protected override async Task UpdateAsync([NotNull] ReviewDto item)
    {
        if (item.Id == -1)
        {
            var review = await repository.AddReviewAsync(
                new Client { Id = item.ClientId },
                new Book { Id = item.BookId },
                item.Score,
                item.Text);
            MessageBox.Show($"Review created with ID={review.Id}");
        }
        else
        {
            await repository.UpdateAsync(new Review
            {
                Id = item.Id,
                Score = item.Score,
                Text = item.Text
            });
        }

        await RefreshAsync();
    }
}