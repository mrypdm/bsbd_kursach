using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Extensions;
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
            Button("Delete", nameof(Delete)),
            Button("Update", nameof(Update)),
            Text(nameof(ReviewDataViewModel.BookId), true),
            Text(nameof(ReviewDataViewModel.Book), true),
            Text(nameof(ReviewDataViewModel.ClientId), true),
            Text(nameof(ReviewDataViewModel.Client), true),
            Text(nameof(ReviewDataViewModel.Score)),
            Text(nameof(ReviewDataViewModel.Text))
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
        Review review;

        if (item.IsNew)
        {
            review = await _repository.AddReviewAsync(
                new Client { Id = item.ClientId },
                new Book { Id = item.BookId },
                item.Score,
                item.Text);
            MessageBox.Show($"Review created for book with ID={review.BookId} for client with ID={review.ClientId}");
        }
        else
        {
            review = await _repository.GetByIdAsync(item.BookId, item.ClientId);
            review.Score = item.Score;
            review.Text = item.Text;

            await _repository.UpdateAsync(review);
        }

        Entities.Replace(item, Mapper.Map<ReviewDataViewModel>(review));
    }

    protected override async Task DeleteAsync([NotNull] ReviewDataViewModel item)
    {
        if (!item.IsNew)
        {
            await _repository.RemoveAsync(new Review { BookId = item.BookId, ClientId = item.ClientId });
        }

        Entities.Remove(item);
    }
}